# SQL Format Extension - Project Documentation

## Project Overview
This is a VS Code extension called "laan.sqlformat" that formats SQL code using an external `sqlformat` command-line tool.

## Requirements
- The `sqlformat` tool must be installed and available in the system PATH
- The tool accepts the following command: `sqlformat -Sql "(sql text)"`

## Project Structure
- `src/extension.ts` - Main extension code
- `package.json` - Extension manifest and configuration
- `tsconfig.json` - TypeScript compiler configuration
- `esbuild.js` - Build configuration
- `.vscode/launch.json` - Debug configuration
- `.vscode/tasks.json` - Build tasks

## Features
- Format selected SQL text or entire file
- Keyboard shortcut: Ctrl+Alt+F (Windows/Linux) or Cmd+Alt+F (Mac)
- Command palette: "SQL Format: Format Selection"
- Error handling with output channel for diagnostics
- Handles non-zero exit codes from sqlformat tool

## Development
1. Install dependencies: `npm install`
2. Compile: `npm run compile`
3. Debug: Press F5 to launch extension host
4. Test: Select SQL text and use Ctrl+Alt+F

## How It Works
1. User selects SQL text (or nothing for entire file)
2. Extension invokes `sqlformat -Sql "(selected text)"`
3. On success: formatted SQL replaces selection
4. On error: error details appear in "SQL Format" output channel
