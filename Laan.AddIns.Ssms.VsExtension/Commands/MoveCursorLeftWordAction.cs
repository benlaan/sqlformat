using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class MoveCursorLeftWordAction : BaseLeftCusorAction
    {
        public override int CommandId => 4006;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CursorLeft(false);
        }

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return base.CanExecute() 
                && CurrentSelection.Length == 0;
        }
    }
}