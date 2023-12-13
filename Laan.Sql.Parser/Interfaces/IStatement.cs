using System;

namespace Laan.Sql.Parser
{
    public interface IStatement
    {
        bool Terminated { get; set; }
        string Identifier { get; }
    }
}
