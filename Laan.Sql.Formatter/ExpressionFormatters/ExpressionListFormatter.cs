using System;
using System.Linq;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class ExpressionListFormatter : CustomExpressionFormatter<ExpressionList>
    {
        public ExpressionListFormatter( ExpressionList expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            return GetIndent( false ) +
                String.Join( ", ", _expression.Identifiers.Select( id => id.FormattedValue( Offset, this ) ).ToArray()
            );
        }

        #endregion
    }
}
