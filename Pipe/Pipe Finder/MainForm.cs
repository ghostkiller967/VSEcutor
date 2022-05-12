using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pipe_Finder
{
    public partial class MainForm : Form
    {
        public FileSystemWatcher FileWatcher = new FileSystemWatcher();
        public bool Listening = false;
        public int MouseX;
        public int MouseY;

        public MainForm()
        {
            InitializeComponent();
            FileWatcher.Path = "\\\\.\\pipe\\";
            FileWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.LastAccess);
            FileWatcher.Created += new FileSystemEventHandler(FileWatcher_Created);
            FileWatcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);
            FileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                if (Listening)
                {
                    string pipe = e.FullPath.Replace("\\\\.\\pipe\\\\", string.Empty);
                    PipesBox.Items.Remove(pipe);
                    PipesBox.Refresh();
                }
            }));
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                if (Listening)
                {
                    string pipe = e.FullPath.Replace("\\\\.\\pipe\\\\", string.Empty);
                    PipesBox.Items.Add(pipe);
                    PipesBox.Refresh();
                }
            }));
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            PipesBox.Items.Clear();
        }

        private void CopyToClipboardButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(PipesBox.Items[PipesBox.IndexFromPoint(new Point(MouseX, MouseY))].ToString());
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Listening = true;
            StopButton.Enabled = true;
            StartButton.Enabled = false;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Listening = false;
            StopButton.Enabled = false;
            StartButton.Enabled = true;
        }

        private void PipesBox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseX = e.X;
            MouseY = e.Y;
        }
    }
}
