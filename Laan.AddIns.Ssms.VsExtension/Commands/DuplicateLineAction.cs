using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class DuplicateLineAction : BaseAction
    {
        public override int CommandId => 4005;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var cursor = TextDocument.Selection.ActivePoint;
            var startPoint = cursor.CreateEditPoint();

            var line = CurrentLine;
            TextDocument.Selection.StartOfLine();
            InsertText(line + Environment.NewLine);
            TextDocument.Selection.Cancel();

            TextDocument.Selection.MoveToPoint(startPoint);
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