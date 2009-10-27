using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Laan.SQL.Parser
{
    public class CreateIndexStatement : IStatement
    {
        public bool Terminated { get; set; }
        public CreateIndexStatement()
        {
            Unique = false;
            Clustered = false;
            Columns = new List<IndexedColumn>();
        }

        public bool Clustered { get; set; }
        public bool Unique { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public List<IndexedColumn> Columns { get; set; }

        #region IStatement Members

        public string Value
        {
            get
            {
                return String.Format(
                    "CREATE INDEX {0}ON {1}({2})",
                    IndexName,
                    TableName,
                    String.Join( ", ", Columns.Select( c => c.Name ).ToArray() )
                );
            }
        }

        #endregion
    }
}
