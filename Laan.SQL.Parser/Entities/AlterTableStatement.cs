using System;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class AlterTableStatement : IStatement
    {
        public bool Terminated { get; set; }
        internal AlterTableStatement()
        {
        }

        public List<string> PrimaryKeys { get; set; }
        public string ConstraintName { get; set; }
        public string TableName { get; set; }

        #region IStatement Members

        public string Value
        {
            get
            {
                return String.Format(
                    "ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY CLUSTERED ({2})",
                    TableName,
                    ConstraintName,
                    String.Join( ", ", PrimaryKeys.ToArray() )
                );
            }
        }

        #endregion
    }
}
