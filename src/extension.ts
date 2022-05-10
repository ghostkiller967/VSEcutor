import vscode from "vscode";
import _open from "open";
import fs from "fs";
import fetch from "node-fetch";

async function download(url: string, dest: string): Promise<void> {
	const response = await fetch(url);
	const fileStream = fs.createWriteStream(dest);
	await new Promise((resolve, reject) => {
		response.body!.pipe(fileStream);
		response.body!.on("error", reject);
		fileStream.on("finish", resolve);
	});
}

export function activate(context: vscode.ExtensionContext): void {

	if (!fs.existsSync("Pipe.exe")) {
		download("https://api.github.com/repos/ghostkiller967/VSEcutor/contents/Pipe.exe", "Pipe.exe");
	}

	context.subscriptions.push(vscode.commands.registerCommand("vsecutor.inject", () => {
		_open("Pipe.exe", {
			app: {
				name: "Pipe.exe",
				arguments: [ "-Attach", vscode.workspace.getConfiguration().get("conf.settingsEditor.dllPath") as string, vscode.workspace.getConfiguration().get("conf.settingsEditor.targetProcess") as string ],
			},
		});
	}));

	context.subscriptions.push(vscode.commands.registerCommand("vsecutor.execute", (uri: vscode.Uri | undefined) => {
		const filePath = uri?.fsPath ?? vscode.window.activeTextEditor?.document.uri.fsPath;
		if (!filePath) { 
			vscode.window.showErrorMessage("You need to have a file open to use this command, this has to be a saved file on the drive.");
			return;
		}
		_open("Pipe.exe", {
			app: {
				name: "Pipe.exe",
				arguments: [ "-Execute", vscode.workspace.getConfiguration().get("conf.settingsEditor.pipe") as string, filePath ],
			},
		});
	}));
}
 
export function deactivate(): void {
	
}