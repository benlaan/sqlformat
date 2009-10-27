using System;

namespace Laan.SQL.Parser
{
    public interface IStatement
    {
        bool Terminated { get; set; }
        string Value { get; }
    }
}
