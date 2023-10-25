interface ISqlFormatter {

    Execute(sq: string) : string;
}

// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';

// @ts-ignore
import { dotnet } from '../../Laan.Sql.Formatter.Wasm.Console/bin/Debug/net7.0/browser-wasm/dotnet.js'

var formatter: ISqlFormatter | null = null;

var v = await dotnet
    .withDiagnosticTracing(true)
    .create();

const config = v.getConfig();
formatter = await v.getAssemblyExports(config.mainAssemblyName || "");

await dotnet.run();

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "laan-addins-vscode-extension" is now active!');

    // The command has been defined in the package.json file
    // Now provide the implementation of the command with registerCommand
    // The commandId parameter must match the command field in package.json
    let disposable = vscode.commands.registerCommand('laan-addins-vscode-extension.helloWorld', () =>
    {
        formatter?.Execute("SELECT * FROM [Table] T WHERE Id = 1");

        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        vscode.window.showInformationMessage('Hello World from Laan.AddIns.VsCode.Extension!');
    });

    context.subscriptions.push(disposable);
}

// This method is called when your extension is deactivated
export function deactivate() {}
