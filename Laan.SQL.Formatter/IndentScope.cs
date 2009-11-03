using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    internal class IndentScope : IDisposable
    {
        private IIndentable _formatter;

        public IndentScope( IIndentable formatter )
        {
            _formatter = formatter;
            _formatter.IncreaseIndent();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _formatter.DecreaseIndent();
        }

        #endregion
    }
}
