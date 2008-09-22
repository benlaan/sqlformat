using System;

namespace Laan.SQL.Parser
{
    public class CreateViewStatement : IStatement
    {
        public SelectStatement SelectBlock { get; set; }
    }
}
