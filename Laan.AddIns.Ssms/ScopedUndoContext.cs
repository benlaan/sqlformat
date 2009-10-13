using System;

namespace Laan.AddIns.Core
{
    internal class ScopedUndoContext : IDisposable
    {
        private AddIn _addIn;

        public ScopedUndoContext( AddIn addIn, string name )
        {
            _addIn = addIn;
            _addIn.OpenUndoContext( name, true );
        }

        #region IDisposable Members

        public void Dispose()
        {
            _addIn.CloseUndoContext();
        }

        #endregion
    }
}
