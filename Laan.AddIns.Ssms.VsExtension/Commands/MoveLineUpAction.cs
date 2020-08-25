using System;
using System.Linq;


using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class MoveLineUpAction : BaseAction
    {
        public override int CommandId => 4003;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var textDocument = TextDocument;
            var cursor = Cursor;

            if (cursor.Row == textDocument.StartPoint.Line)
                return;

            var line = CurrentLine;
            textDocument.Selection.LineUp();
            var otherLine = CurrentLine;

            textDocument.Selection.SelectLine();
            InsertText(line + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            var y = textDocument.Selection.Text;

            textDocument.Selection.LineDown();
            textDocument.Selection.SelectLine();
            InsertText(otherLine + Environment.NewLine, true);

            textDocument.Selection.Cancel();
            textDocument.Selection.LineUp();
            textDocument.Selection.CharRight(false, cursor.Column - 1);
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && AllText.Length > 0 
                && CurrentSelection.Length == 0;
        }
    }
}