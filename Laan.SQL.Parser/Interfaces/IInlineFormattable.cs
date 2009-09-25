using System;

namespace Laan.SQL.Parser.Expressions
{
    public interface IInlineFormattable
    {
        bool CanInline { get; }
    }
}
