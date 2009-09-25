using System;

namespace Laan.SQL.Parser
{
    public class UpdateStatement : ProjectionStatement
    {
        public Top Top { get; set; }
        public string TableName { get; set; }
    }
}