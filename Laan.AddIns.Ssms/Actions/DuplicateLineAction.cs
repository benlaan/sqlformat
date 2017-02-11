using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    [Menu("Laan")]
    public class DuplicateLineAction : Core.BaseAction
    {
        public DuplicateLineAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlDuplicateLineAction";
            DisplayName = "Duplicate Line";
            DescriptivePhrase = "Duplicates the current line";

            ButtonText = "Duplicate Line";
            ToolTip = "Duplicate Line";
            KeyboardBinding = "Text Editor::Shift+Enter";
            ImageIndex = 2;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;
            var cursor = textDocument.Selection.ActivePoint;
            var startPoint = cursor.CreateEditPoint();

            var line = AddIn.CurrentLine;
            textDocument.Selection.StartOfLine();
            AddIn.InsertText(line + Environment.NewLine);
            textDocument.Selection.Cancel();

            textDocument.Selection.MoveToPoint(startPoint);
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql")
                && AddIn.AllText.Length > 0 
                && AddIn.CurrentSelection.Length == 0;
        }
    }
}