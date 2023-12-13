using System;

namespace Laan.Sql.Formatter
{
    public interface IStatementFormatter
    {
        void Execute();
        bool CanInline { get; }
    }
}
