using System;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    internal class IndentScope : IDisposable
    {
        private IIndentable _formatter;

        public IndentScope( IIndentable formatter )
        {
            _formatter = formatter;
            _formatter.Indent();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _formatter.Unindent();
        }

        #endregion
    }
}
