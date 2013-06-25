using System;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class CriteriaExpressionFormatter : CustomExpressionFormatter<CriteriaExpression>
    {
        /// <summary>
        /// Initializes a new instance of the CriteriaExpressionFormatter class.
        /// </summary>
        public CriteriaExpressionFormatter( CriteriaExpression expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( !String.Equals(_expression.Operator, Constants.And, StringComparison.InvariantCultureIgnoreCase) && 
                 !String.Equals(_expression.Operator, Constants.Or, StringComparison.InvariantCultureIgnoreCase) )
                return
                    String.Format(
                    "{0} {1} {2}",
                    _expression.Left.FormattedValue( Offset, this ),
                    _expression.Operator,
                    _expression.Right.FormattedValue( Offset, this )
                );

            // this code ensures the boolean expression is indented once
            // ie.
            // (
            // 
            //     A.ID IS NULL
            //     OR
            //     A.ID = 10
            //
            // )
            if ( _expression.Parent is NestedExpression )
            {
                return String.Format(
                    "{0}{1}{2}{3}{2}{4}",
                    GetIndent( false ),
                    _expression.Left.FormattedValue( Offset, this ),
                    GetIndent( true ),
                    _expression.Operator,
                    _expression.Right.FormattedValue( Offset, this )
                );
            }

            // this code ensures that chained boolean expressions are continued at the current nest level
            // ie.
            // (
            // 
            //     A.ID IS NULL
            //     OR
            //     A.ID = 10
            //     OR
            //     A.ID = 20
            //
            // )
            if ( _expression.Parent is CriteriaExpression && _expression.HasAncestorOfType( typeof( NestedExpression ) ) )
            {
                return String.Format(
                    "{0}{1}{2}{1}{3}",
                    _expression.Left.FormattedValue( Offset, this ),
                    GetIndent( true ),
                    _expression.Operator,
                    _expression.Right.FormattedValue( Offset, this )
                );
            }

            // this is the default format, and is used by JOIN, WHERE (non nested), and HAVING criteria
            return String.Format(
                "{0}{1}{2}{3} {4}",
                _expression.Left.FormattedValue( Offset, this ),
                GetIndent( true ),
                new string( ' ', Math.Max( 0, Offset - _expression.Operator.Length ) ),
                _expression.Operator,
                _expression.Right.FormattedValue( Offset, this )
            );
        }

        #endregion
    }
}
