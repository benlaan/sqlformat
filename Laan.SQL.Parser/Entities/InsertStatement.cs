using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class InsertStatement : InsertDeleteStatementBase
    {
        public string TableName { get; set; }
        public bool HasInto { get; set; }
        public List<string> Columns { get; set; }
        public List<List<string>> Values { get; set; }
        public SelectStatement SourceStatement { get; set; }
        internal InsertStatement()
        {
            Columns = new List<string>();
            Values = new List<List<string>>();
        }
    }
}
