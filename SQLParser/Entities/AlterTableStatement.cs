using System;

namespace Laan.SQL.Parser
{
    public class AlterTableStatement : IStatement
    {
        internal AlterTableStatement()
        {
        }

        public string PrimaryKey { get; set; }
        public string ConstraintName { get; set; }
        public string TableName { get; set; }
    }
}
