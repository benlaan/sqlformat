using System.Collections.Generic;
using System;
using System.Diagnostics;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{

    public class Table : AliasedEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table()
        {
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
