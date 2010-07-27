using System.Collections.Generic;
using System;
using System.Diagnostics;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{

    public class Table : AliasedEntity
    {
        public string Name { get; set; }
        public List<Join> Joins { get; set; }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table()
        {
            Joins = new List<Join>();
        }

        public override string Value
        {
            get { return Alias != null ? Name : String.Format( "{0} ({1})", Name, Alias.Name ); }
        }
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
