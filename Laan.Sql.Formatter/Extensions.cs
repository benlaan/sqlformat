using System;
using System.ComponentModel;
using System.Reflection;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public static class Extensions
    {
        public static string FormattedValue(this Expression expr, int offset, IIndentable indent)
        {
            var formatter = ExpressionFormatterFactory.GetFormatter(indent, expr);
            formatter.Offset = offset;
            (formatter as IIndentable).Indent = indent.Indent;
            (formatter as IIndentable).IndentLevel = indent.IndentLevel;
            return formatter.Execute();
        }

        public static bool HasAncestorOfType(this Expression expr, Type type)
        {
            while (expr.Parent != null)
            {
                expr = expr.Parent;
                if (type.IsAssignableFrom(expr.GetType()))
                    return true;
            }
            return false;
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString(); // Fallback to the enum name if no description is found
        }
    }
}