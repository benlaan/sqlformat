using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class InsertDeleteStatementBase : CustomStatement, ITableHints
    {
        internal InsertDeleteStatementBase()
        {
            TableHints = new List<TableHint>();
        }

        public List<TableHint> TableHints { get; set; }
        public bool ExplicitWith { get; set; }
    }
}