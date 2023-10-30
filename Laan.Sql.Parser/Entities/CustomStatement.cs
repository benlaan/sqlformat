using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class CustomStatement : Statement
    {
        public CustomStatement()
        {
            From = new List<Table>();
        }

        public List<Table> From { get; set; }
        public Pivot Pivot { get; set; }
        public Expression Where { get; set; }
    }
}