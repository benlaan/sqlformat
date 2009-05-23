using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.SQL.Parser
{
    public class CreateIndex : IStatement
    {
        public CreateIndex()
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
    }
}
