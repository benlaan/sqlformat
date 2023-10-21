using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class Top
    {
        public Top(Expression value)
        {
            Expression = value;
        }

        public override string ToString()
        {
            return String.Format(" TOP {0}{1}", Expression.Value, Percent ? " PERCENT " : "");
        }

        public Expression Expression { get; set; }
        public bool Percent { get; set; }
    }
}
