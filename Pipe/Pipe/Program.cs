using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pipe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args = args.Skip(1).ToArray();
            if (args.Length > 0)
            {
                if (args[0] == "-Attach")
                {
                    Console.WriteLine("Attaching to " + args[2]);
                    InjectDLL(args[1], args[2]);
                }
                else if (args[0] == "-Execute")
                {
                    Console.WriteLine("Executing...");
                    Execute(args[1], File.ReadAllText(args[2]));
                }
            }
            else
            {
                Console.WriteLine("No arguments given.");
            }
        }

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }

        #endregion

        #region Enums

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        #endregion

        #region Win32 API

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        private static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        #endregion

        #region Constants

        public const int MAX_PATH = 260;
        private const int INVALID_HANDLE_VALUE = -1;

        #endregion

        #region Functions

        #region Private Functions

        private static IntPtr GetModuleBaseAddress(int procId, string modName)
        {
            IntPtr modBaseAddr = IntPtr.Zero;

            IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, procId);

            if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
            {
                MODULEENTRY32 modEntry = new MODULEENTRY32
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32))
                };

                if (Module32First(hSnap, ref modEntry))
                {
                    do
                    {
                        if (modEntry.szModule.Equals(modName))
                        {
                            modBaseAddr = modEntry.modBaseAddr;
                            break;
                        }
                    } while (Module32Next(hSnap, ref modEntry));
                }
            }

            CloseHandle(hSnap);
            return modBaseAddr;
        }

        #endregion

        #region Public Functions

        public static bool InjectDLL(string dllPath, string procName)
        {
            Process[] procs = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(procName));

            if (procs.Length == 0)
            {
                return false;
            }

            Process proc = procs[0];

            if (proc.Handle != IntPtr.Zero)
            {
                IntPtr loc = VirtualAllocEx(proc.Handle, IntPtr.Zero, MAX_PATH, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);

                if (loc.Equals(0))
                {
                    return false;
                }

                if (!WriteProcessMemory(proc.Handle, loc, dllPath.ToCharArray(), dllPath.Length, out IntPtr bytesRead) || bytesRead.Equals(0))
                {
                    return false;
                }

                IntPtr loadlibAddy = GetProcAddress(GetModuleBaseAddress(proc.Id, "KERNEL32.DLL"), "LoadLibraryA");
                IntPtr hThread = CreateRemoteThread(proc.Handle, IntPtr.Zero, 0, loadlibAddy, loc, 0, out _);

                if (!hThread.Equals(0))
                {
                    CloseHandle(hThread);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            proc.Dispose();
            return true;
        }

        public static void Execute(string pipeName, string script)
        {
            if (NamedPipeExist(pipeName))
            {
                try
                {
                    using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                    {
                        namedPipeClientStream.Connect();
                        using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
                        {
                            streamWriter.Write(script);
                            streamWriter.Dispose();
                        }
                        namedPipeClientStream.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("You haven't injected the dll yet.");
            }
        }

        public static bool NamedPipeExist(string pipeName)
        {
            try
            {
                if (!WaitNamedPipe(Path.GetFullPath("\\\\.\\pipe\\" + pipeName), 0))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0 || lastWin32Error == 2)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #endregion

    }
}
