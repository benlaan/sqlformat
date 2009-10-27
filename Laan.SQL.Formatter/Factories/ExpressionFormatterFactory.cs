using System;
using System.Collections.Generic;

using Laan.SQL.Parser;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public class ExpressionFormatterFactory
    {
        private static Dictionary<Type, Type> _formatters;

        static ExpressionFormatterFactory()
        {
            _formatters = new Dictionary<Type, Type>
            {
                { typeof( CriteriaExpression ), typeof( CriteriaExpressionFormatter ) },
                { typeof( CaseSwitchExpression ), typeof( CaseSwitchExpressionFormatter ) },
                { typeof( CaseWhenExpression ), typeof( CaseWhenExpressionFormatter ) },
                { typeof( FunctionExpression ), typeof( FunctionExpressionFormatter ) },
                { typeof( NestedExpression ), typeof( NestedExpressionFormatter ) },
                { typeof( SelectExpression ), typeof( SelectExpressionFormatter ) },
                { typeof( ExpressionList ), typeof( ExpressionListFormatter ) },
                { typeof( BetweenExpression ), typeof( BetweenExpressionFormatter ) },
            };
        }

        public static IExpressionFormatter GetFormatter( IIndentable indentable, Expression expression )
        {
            Type formatterType;
            if ( !_formatters.TryGetValue( expression.GetType(), out formatterType ) )
                return new DefaultExpressionFormatter( expression );

            var formatter = Activator.CreateInstance( formatterType, expression ) as IExpressionFormatter;
            ( formatter as IIndentable ).IndentLevel = indentable.IndentLevel;
            ( formatter as IIndentable ).Indent = indentable.Indent;

            if ( formatter == null )
                throw new ArgumentNullException( "Formatter not instantiated: " + formatterType.Name );

            return formatter;
        }
    }
}
