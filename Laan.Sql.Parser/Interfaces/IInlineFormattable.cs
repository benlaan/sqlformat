using System;

namespace Laan.Sql.Parser.Expressions
{
    public interface IInlineFormattable
    {
        bool CanInline { get; }
    }
}
