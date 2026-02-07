import * as vscode from 'vscode';
import { execFile } from 'child_process';
import { promisify } from 'util';
import * as path from 'path';
import * as fs from 'fs';

const execFileAsync = promisify(execFile);

let outputChannel: vscode.OutputChannel;

interface SqlFormattingOptions {
	indentSize?: number;
	useSpaces?: boolean;
	maxLineLength?: number;
	keywordCasing?: string;
	bracketSpacing?: string;
	blankLinesBetweenClauses?: number;
	maxInlineSelectColumns?: number;
	maxInlineInsertColumns?: number;
	configFile?: string;
}

export function activate(context: vscode.ExtensionContext) {
	// Create output channel for errors
	outputChannel = vscode.window.createOutputChannel('SQL Format');
	context.subscriptions.push(outputChannel);

	// Register the format command
	const disposable = vscode.commands.registerTextEditorCommand(
		'laan.sqlformat.formatSelection',
		async (textEditor: vscode.TextEditor, edit: vscode.TextEditorEdit) => {
			await formatSelection(textEditor, context);
		}
	);

	context.subscriptions.push(disposable);

	// Register as document formatter for SQL files
	context.subscriptions.push(
		vscode.languages.registerDocumentFormattingEditProvider(
			{ scheme: 'file', language: 'sql' },
			new SqlFormattingProvider(context)
		)
	);

	// Also register for untitled SQL files
	context.subscriptions.push(
		vscode.languages.registerDocumentFormattingEditProvider(
			{ scheme: 'untitled', language: 'sql' },
			new SqlFormattingProvider(context)
		)
	);
}

/**
 * Get the path to the sqlformat executable.
 * First tries to use the bundled binary, then falls back to PATH.
 */
function getSqlFormatPath(context: vscode.ExtensionContext): string | null {
	const platform = process.platform;
	const arch = process.arch;
	
	// Determine runtime identifier
	let rid: string;
	if (platform === 'win32') {
		rid = 'win-x64';
	} else if (platform === 'darwin') {
		rid = arch === 'arm64' ? 'osx-arm64' : 'osx-x64';
	} else {
		// Assume linux
		rid = 'linux-x64';
	}
	
	const exeName = platform === 'win32' ? 'sqlformat.exe' : 'sqlformat';
	const bundledPath = context.asAbsolutePath(path.join('bin', rid, exeName));
	
	// Check if bundled binary exists
	if (fs.existsSync(bundledPath)) {
		return bundledPath;
	}
	
	// Fall back to PATH
	return 'sqlformat';
}

/**
 * Get formatting options from VS Code configuration
 */
function getFormattingOptions(): SqlFormattingOptions {
	const config = vscode.workspace.getConfiguration('sqlformat');
	
	return {
		indentSize: config.get<number>('indentSize'),
		useSpaces: config.get<boolean>('useSpaces'),
		maxLineLength: config.get<number>('maxLineLength'),
		keywordCasing: config.get<string>('keywordCasing'),
		bracketSpacing: config.get<string>('bracketSpacing'),
		blankLinesBetweenClauses: config.get<number>('blankLinesBetweenClauses'),
		maxInlineSelectColumns: config.get<number>('maxInlineSelectColumns'),
		maxInlineInsertColumns: config.get<number>('maxInlineInsertColumns'),
		configFile: config.get<string>('configFile')
	};
}

/**
 * Find .sqlformat.json file in workspace hierarchy
 */
function findConfigFile(): string | null {
	const workspaceFolders = vscode.workspace.workspaceFolders;
	if (!workspaceFolders || workspaceFolders.length === 0) {
		return null;
	}
	
	const workspaceRoot = workspaceFolders[0].uri.fsPath;
	let currentDir = workspaceRoot;
	
	// Search upward from workspace root
	while (currentDir) {
		const configPath = path.join(currentDir, '.sqlformat.json');
		if (fs.existsSync(configPath)) {
			return configPath;
		}
		
		const parentDir = path.dirname(currentDir);
		if (parentDir === currentDir) {
			break; // Reached root
		}
		currentDir = parentDir;
	}
	
	return null;
}

/**
 * Build command-line arguments from formatting options
 */
function buildCommandArgs(text: string, options: SqlFormattingOptions): string[] {
	const args: string[] = ['-Sql', text];
	
	// Check for config file first
	if (options.configFile && options.configFile.trim().length > 0) {
		// Explicit config file specified
		args.push('-ConfigFile', options.configFile);
		return args;
	}
	
	// Check for auto-discovered config file
	const autoConfigFile = findConfigFile();
	if (autoConfigFile) {
		args.push('-ConfigFile', autoConfigFile);
		return args;
	}
	
	// Otherwise, use individual options from VS Code settings
	if (options.indentSize !== undefined) {
		args.push('-IndentSize', options.indentSize.toString());
	}
	
	if (options.useSpaces !== undefined) {
		if (options.useSpaces) {
			args.push('-UseSpaces');
		} else {
			args.push('-UseTabs');
		}
	}
	
	if (options.maxLineLength !== undefined) {
		args.push('-MaxLineLength', options.maxLineLength.toString());
	}
	
	if (options.keywordCasing) {
		args.push('-KeywordCasing', options.keywordCasing);
	}
	
	if (options.bracketSpacing) {
		args.push('-BracketSpacing', options.bracketSpacing);
	}
	
	return args;
}

async function formatSelection(textEditor: vscode.TextEditor, context: vscode.ExtensionContext): Promise<void> {
	const selection = textEditor.selection;
	
	// Get selected text or entire document if nothing is selected
	const text = textEditor.document.getText(
		selection.isEmpty ? undefined : selection
	);

	if (!text || text.trim().length === 0) {
		vscode.window.showWarningMessage('No text selected to format');
		return;
	}

	const sqlFormatPath = getSqlFormatPath(context);
	if (!sqlFormatPath) {
		outputChannel.appendLine('ERROR: sqlformat binary not found');
		vscode.window.showErrorMessage('sqlformat binary not found in extension or PATH');
		outputChannel.show(true);
		return;
	}

	const sqlOptions = getFormattingOptions();

	try {
		// Execute sqlformat with formatting options
		const args = buildCommandArgs(text, sqlOptions);
		const { stdout, stderr } = await execFileAsync(sqlFormatPath, args, {
			timeout: 30000, // 30 second timeout
			maxBuffer: 10 * 1024 * 1024 // 10MB buffer
		});

		// Replace the selection with the formatted output
		await textEditor.edit(editBuilder => {
			if (selection.isEmpty) {
				// Replace entire document
				const fullRange = new vscode.Range(
					textEditor.document.positionAt(0),
					textEditor.document.positionAt(textEditor.document.getText().length)
				);
				editBuilder.replace(fullRange, stdout);
			} else {
				// Replace selection
				editBuilder.replace(selection, stdout);
			}
		});

		// Show success message if there were warnings in stderr
		if (stderr && stderr.trim().length > 0) {
			outputChannel.appendLine('Formatted successfully with warnings:');
			outputChannel.appendLine(stderr);
			vscode.window.showWarningMessage('SQL formatted with warnings. Check output for details.');
		}

	} catch (error: any) {
		// Handle errors - show in output channel
		outputChannel.clear();
		outputChannel.appendLine('SQL Format Error:');
		outputChannel.appendLine('─'.repeat(50));
		
		if (error.code === 'ENOENT') {
			outputChannel.appendLine('ERROR: sqlformat command not found in PATH');
			outputChannel.appendLine('Please ensure the sqlformat tool is installed and available in your system PATH.');
			vscode.window.showErrorMessage('sqlformat tool not found in PATH');
		}
        else if (error.code === 'ETIMEDOUT') {
			outputChannel.appendLine('ERROR: sqlformat command timed out');
			vscode.window.showErrorMessage('SQL formatting timed out');
		}
        else {
			// Show exit code and error output
			if (error.code) {
				outputChannel.appendLine(`Exit Code: ${error.code}`);
			}
			if (error.stderr) {
				outputChannel.appendLine('Standard Error:');
				outputChannel.appendLine(error.stderr);
			}
			if (error.stdout) {
				outputChannel.appendLine('Standard Output:');
				outputChannel.appendLine(error.stdout);
			}
			if (error.message) {
				outputChannel.appendLine('Error Message:');
				outputChannel.appendLine(error.message);
			}
			
			vscode.window.showErrorMessage('SQL formatting failed. Check output for details.');
		}
		
		outputChannel.show(true);
	}
}

/**
 * Document formatting provider for SQL files
 */
class SqlFormattingProvider implements vscode.DocumentFormattingEditProvider {
	constructor(private context: vscode.ExtensionContext) {}

	async provideDocumentFormattingEdits(
		document: vscode.TextDocument,
		options: vscode.FormattingOptions,
		token: vscode.CancellationToken
	): Promise<vscode.TextEdit[]> {
		const text = document.getText();

		if (!text || text.trim().length === 0) {
			return [];
		}

		const sqlFormatPath = getSqlFormatPath(this.context);
		if (!sqlFormatPath) {
			outputChannel.appendLine('ERROR: sqlformat binary not found');
			vscode.window.showErrorMessage('sqlformat binary not found in extension or PATH');
			outputChannel.show(true);
			return [];
		}

		const sqlOptions = getFormattingOptions();

		try {
			const args = buildCommandArgs(text, sqlOptions);
			const { stdout } = await execFileAsync(sqlFormatPath, args, {
				timeout: 30000,
				maxBuffer: 10 * 1024 * 1024
			});

			// Return edit that replaces entire document
			const fullRange = new vscode.Range(
				document.positionAt(0),
				document.positionAt(text.length)
			);

			return [vscode.TextEdit.replace(fullRange, stdout)];
		} catch (error: any) {
			// Handle errors
			outputChannel.clear();
			outputChannel.appendLine('SQL Format Error:');
			outputChannel.appendLine('─'.repeat(50));

			if (error.code === 'ENOENT') {
				outputChannel.appendLine('ERROR: sqlformat command not found');
				vscode.window.showErrorMessage('sqlformat tool not found');
			} else if (error.stderr) {
				outputChannel.appendLine('Error output:');
				outputChannel.appendLine(error.stderr);
				vscode.window.showErrorMessage('SQL formatting failed. Check output for details.');
			} else {
				outputChannel.appendLine(error.message || String(error));
				vscode.window.showErrorMessage('SQL formatting failed');
			}

			outputChannel.show(true);
			return [];
		}
	}
}

export function deactivate() {
	if (outputChannel) {
		outputChannel.dispose();
	}
}
