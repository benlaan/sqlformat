using System.Collections.Generic;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Entities
{
    public class InsertDeleteStatementBase : CustomStatement, ITableHints
    {
        internal InsertDeleteStatementBase()
        {
            TableHints = new List<TableHint>();
        }

        public List<TableHint> TableHints
        {
            get; set; }
    }
}