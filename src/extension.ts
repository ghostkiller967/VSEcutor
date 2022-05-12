import vscode from 'vscode';
import _open from "open";
import fs from "fs";
import fetch from "node-fetch";
import http from "https";

interface Version {
	version: string;
}

export function activate(context: vscode.ExtensionContext) {

	context.subscriptions.push(vscode.commands.registerCommand("vsecutor.inject", () => {
		_open("Pipe.exe", {
			app: {
				name: "Pipe.exe",
				arguments: ["-Attach", vscode.workspace.getConfiguration().get("conf.settingsEditor.dllPath") as string, vscode.workspace.getConfiguration().get("conf.settingsEditor.targetProcess") as string],
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
				arguments: ["-Execute", vscode.workspace.getConfiguration().get("conf.settingsEditor.pipe") as string, filePath],
			},
		});
	}));

	context.subscriptions.push(vscode.commands.registerCommand("vsecutor.update", async () => {
		const response = await fetch("https://raw.githubusercontent.com/ghostkiller967/VSEcutor/main/PipeVersion.json");
		const latestVersion: Version = JSON.parse(await response.text());
		if (fs.existsSync("PipeVersion.json")) {
			const currentVersion: Version = JSON.parse(fs.readFileSync("PipeVersion.json", "utf8"));
			if (currentVersion.version !== latestVersion.version) {
				fs.writeFileSync("PipeVersion.json", JSON.stringify({ "version": latestVersion.version }));
				const file = fs.createWriteStream("Pipe.exe");
				http.get("https://raw.githubusercontent.com/ghostkiller967/VSEcutor/main/Pipe.exe", function(response) {
					response.pipe(file);
				});
			}
		} else {
			fs.writeFileSync("PipeVersion.json", JSON.stringify({ "version": latestVersion.version }));
			http.get("https://raw.githubusercontent.com/ghostkiller967/VSEcutor/main/Pipe.exe", function(response) {
				response.pipe(fs.createWriteStream("Pipe.exe"));
			});
		}
	}));
}

export function deactivate() {}
