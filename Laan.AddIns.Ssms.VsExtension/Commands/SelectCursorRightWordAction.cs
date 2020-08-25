using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SelectCursorRightWordAction : BaseRightCusorAction
    {
        public override int CommandId => 4009;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CursorRight(true);
        }
    }
}