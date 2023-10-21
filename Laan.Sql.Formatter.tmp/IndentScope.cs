using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    internal class IndentScope : IDisposable
    {
        private IIndentable _formatter;
        private bool _indent;

        public IndentScope( IIndentable formatter ) : this( formatter, true )
        {
        }

        public IndentScope( IIndentable formatter, bool indent )
        {
            _indent = indent;
            _formatter = formatter;

            if ( _indent )
                _formatter.IncreaseIndent();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if ( _indent )
                _formatter.DecreaseIndent();
        }

        #endregion
    }
}
