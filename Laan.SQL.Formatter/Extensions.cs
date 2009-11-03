using System;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public static class Extensions
    {
        public static string FormattedValue( this Expression expr, int offset, IIndentable indent )
        {
            var formatter = ExpressionFormatterFactory.GetFormatter( indent, expr );
            formatter.Offset = offset;
            ( formatter as IIndentable ).Indent = indent.Indent;
            ( formatter as IIndentable ).IndentLevel = indent.IndentLevel;
            return formatter.Execute();
        }

        public static bool HasAncestorOfType( this Expression expr, Type type )
        {
            while ( expr.Parent != null )
            {
                expr = expr.Parent;
                if ( type.IsAssignableFrom( expr.GetType() ) )
                    return true;
            }
            return false;
        }

    }
}
