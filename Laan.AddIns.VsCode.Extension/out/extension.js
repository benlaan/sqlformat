"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.deactivate = exports.activate = void 0;
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
const vscode = require("vscode");
// @ts-ignore
const dotnet_js_1 = require("../../Laan.Sql.Formatter.Wasm.Console/bin/Debug/net7.0/browser-wasm/dotnet.js");
var formatter = null;
var v = await dotnet_js_1.dotnet
    .withDiagnosticTracing(true)
    .create();
const config = v.getConfig();
formatter = await v.getAssemblyExports(config.mainAssemblyName || "");
await dotnet_js_1.dotnet.run();
// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
function activate(context) {
    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "laan-addins-vscode-extension" is now active!');
    // The command has been defined in the package.json file
    // Now provide the implementation of the command with registerCommand
    // The commandId parameter must match the command field in package.json
    let disposable = vscode.commands.registerCommand('laan-addins-vscode-extension.helloWorld', () => {
        formatter?.Execute("SELECT * FROM [Table] T WHERE Id = 1");
        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        vscode.window.showInformationMessage('Hello World from Laan.AddIns.VsCode.Extension!');
    });
    context.subscriptions.push(disposable);
}
exports.activate = activate;
// This method is called when your extension is deactivated
function deactivate() { }
exports.deactivate = deactivate;
//# sourceMappingURL=extension.js.map