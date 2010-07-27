using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class CustomExpressionFormatter<T> : BaseFormatter, IExpressionFormatter, IIndentable where T : Expression
    {
        private const int MaxColumnWidth = 80;
        private const int TabSize = 4;

        protected T _expression;

        public CustomExpressionFormatter( T expression )
        {
            Indent = GetSpaces(TabSize);
            _expression = expression;
        }

        protected bool CanInlineExpression( Expression expr, int offset )
        {
            return
                expr is IInlineFormattable &&
                ( (IInlineFormattable) expr ).CanInline &&
                expr.Value.Length < MaxColumnWidth - offset;
        }
        
        protected int GetCurrentColumn( StringBuilder sql )
        {
            return sql.ToString().Split( '\n' ).Last().Length;
        }

        protected string GetIndent( bool includeNewLine )
        {
            var result = new StringBuilder( includeNewLine ? "\r\n" : "" );

            for ( int index = 0; index < IndentLevel; index++ )
                result.Append( Indent );
            
            return result.ToString();
        }

        protected static string GetSpaces( int offset )
        {
            return new string( ' ', offset );
        }

        #region IExpressionFormatter Members

        public virtual string Execute()
        {
            return _expression.Value;
        }

        public int Offset { get; set; }

        #endregion

        #region IIndentable Members

        public string Indent { get; set; }
        public int IndentLevel { get; set; }

        public void IncreaseIndent()
        {
            IndentLevel++;
        }

        public void DecreaseIndent()
        {
            IndentLevel--;
        }

        #endregion
    }
}
