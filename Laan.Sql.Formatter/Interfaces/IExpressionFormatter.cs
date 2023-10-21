using System;

namespace Laan.Sql.Formatter
{
    public interface IExpressionFormatter
    {
        int Offset { get; set; }
        string Execute();
    }
}
