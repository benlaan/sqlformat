using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class MoveCursorRightWordAction : BaseRightCusorAction
    {
        public override int CommandId => 4007;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CursorRight(false);
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.CanExecute() 
                && CurrentSelection.Length == 0;
        }
    }
}