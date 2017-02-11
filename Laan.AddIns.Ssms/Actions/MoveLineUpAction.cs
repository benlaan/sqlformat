using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class MoveLineUpAction : Core.BaseAction
    {
        public MoveLineUpAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlMoveLineUpAction";
            DisplayName = "Move Line Up";
            DescriptivePhrase = "Moves the current line up";

            ButtonText = DisplayName;
            ToolTip = DisplayName;
            KeyboardBinding = "Text Editor::Alt+Up Arrow";
            ImageIndex = 3;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;
            var cursor = AddIn.Cursor;

            if (cursor.Row == textDocument.StartPoint.Line)
                return;

            var line = AddIn.CurrentLine;
            textDocument.Selection.LineUp();
            var otherLine = AddIn.CurrentLine;

            textDocument.Selection.SelectLine();
            AddIn.InsertText(line + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            var y = textDocument.Selection.Text;

            textDocument.Selection.LineDown();
            textDocument.Selection.SelectLine();
            AddIn.InsertText(otherLine + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            textDocument.Selection.LineUp();
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