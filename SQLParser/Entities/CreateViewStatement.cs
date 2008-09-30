using System;

namespace Laan.SQL.Parser
{
    public class CreateViewStatement : IStatement
    {
        public SelectStatement SelectBlock { get; set; }
        public string Name { get; set; }
    }
}
