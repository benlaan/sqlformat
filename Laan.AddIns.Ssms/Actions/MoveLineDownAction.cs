using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class MoveLineDownAction : Core.BaseAction
    {
        public MoveLineDownAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlMoveLineDownAction";
            DisplayName = "Move Line Down";
            DescriptivePhrase = "Moves the current line Down";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Alt+Down Arrow";
            ImageIndex = 4;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;
            var cursor = AddIn.Cursor;

            if (cursor.Row == textDocument.EndPoint.Line)
                return;

            var line = AddIn.CurrentLine;
            textDocument.Selection.LineDown();
            var otherLine = AddIn.CurrentLine;

            textDocument.Selection.SelectLine();
            AddIn.InsertText(line + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            var y = textDocument.Selection.Text;

            textDocument.Selection.LineUp();
            textDocument.Selection.SelectLine();
            AddIn.InsertText(otherLine + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            textDocument.Selection.LineDown();
            textDocument.Selection.CharRight(false, cursor.Column - 1);
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql")
                && AddIn.AllText.Length > 0 
                && AddIn.CurrentSelection.Length == 0;
        }
    }
}