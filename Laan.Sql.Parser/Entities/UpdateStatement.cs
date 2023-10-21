using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Entities
{
    public class UpdateStatement : ProjectionStatement, ITableHints
    {
        public Top Top { get; set; }
        public string TableName { get; set; }
        public List<TableHint> TableHints { get; set; }
        public bool ExplicitWith { get; set; }

        public UpdateStatement()
        {
            TableHints = new List<TableHint>();
        }
    }
}