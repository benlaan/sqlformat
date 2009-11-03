using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class Top
    {
        public Top( Expression value, bool brackets )
        {
            Brackets = brackets;
            Expression = value;
        }

        public override string ToString()
        {
            string format = Brackets ? " TOP ({0}){1}" : " TOP {0}{1}";
            return String.Format( format, Expression.Value, Percent ? " PERCENT " : "" );
        }

        public bool Brackets { get; private set; }
        public Expression Expression { get; set; }
        public bool Percent { get; set; }
    }
}
