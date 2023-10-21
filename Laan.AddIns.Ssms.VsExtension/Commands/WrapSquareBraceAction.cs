using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class WrapSquareBraceAction : BaseAction
    {
        public override int CommandId => 4003;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TextDocument.Selection.Text = String.Format("[{0}]", TextDocument.Selection.Text);
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && CurrentSelection.Length > 0;
        }
    }
}