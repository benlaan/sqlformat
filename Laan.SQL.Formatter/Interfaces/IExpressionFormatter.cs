using System;

namespace Laan.SQL.Formatter
{
    public interface IExpressionFormatter
    {
        int Offset { get; set; }
        string Execute();
    }
}
