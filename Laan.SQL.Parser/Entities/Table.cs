using System.Collections.Generic;
using System;
using System.Diagnostics;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{

    public class Table : AliasedEntity, ITableHints
    {
        public string Name { get; set; }
        public List<Join> Joins { get; set; }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table()
        {
            Joins = new List<Join>();
            TableHints = new List<TableHint>();
        }

        public override string Value
        {
            get { return Alias != null ? Name : String.Format( "{0} ({1})", Name, Alias.Name ); }
        }
        public List<TableHint> TableHints { get; set; }
    }

    public class DerivedTable : Table
    {
        public SelectStatement SelectStatement { get; set; }

        public override string Value
        {
            get
            {
                return "(" + SelectStatement.ToString() + ")";
            }
        }
    }
}
