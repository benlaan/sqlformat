using System;
using System.Collections.Generic;

namespace Laan.SQL.Parser
{
    public class AlterTableStatement : IStatement
    {
        internal AlterTableStatement()
        {
        }

        public List<string> PrimaryKeys { get; set; }
        public string ConstraintName { get; set; }
        public string TableName { get; set; }
    }
}
