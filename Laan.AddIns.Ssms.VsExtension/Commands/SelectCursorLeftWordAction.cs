using System;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SelectCursorLeftWordAction : BaseLeftCusorAction
    {
        public override int CommandId => 4008;

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CursorLeft(true);
        }
    }
}