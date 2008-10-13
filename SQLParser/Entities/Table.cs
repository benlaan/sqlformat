using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Laan.SQL.Parser
{

    public class Table
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public virtual string Value
        {
            get
            {
                return String.IsNullOrEmpty( Alias ) ? Name : String.Format( "{0} ({1})", Name, Alias );
            }
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
