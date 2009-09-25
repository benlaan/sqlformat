using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.SQL.Parser
{
    public class InsertStatement : IStatement
    {
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public List<List<string>> Values { get; set; }
        public SelectStatement SourceStatement { get; set; }
        public bool Terminated { get; set; }

        internal InsertStatement()
        {
            Columns = new List<string>();
            Values = new List<List<string>>();
        }
    }
}
