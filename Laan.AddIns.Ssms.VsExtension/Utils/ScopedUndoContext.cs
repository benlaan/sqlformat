using System;
using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Utils
{
    internal class ScopedUndoContext : IDisposable
    {
        private BaseAction _action;

        public ScopedUndoContext(BaseAction addIn, string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _action = addIn;
            _action.OpenUndoContext(name, true);
        }

        public void Dispose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _action.CloseUndoContext();
        }
    }
}
