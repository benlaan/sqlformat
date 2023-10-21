using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class WrapBracketAction : BaseAction
    {
        public override int CommandId => 4004;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TextDocument.Selection.Text = String.Format("({0})", TextDocument.Selection.Text);
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && CurrentSelection.Length > 0;
        }
    }
}