using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.Sql.Parser.Expressions
{
    public class FunctionExpression : Expression
    {
        public FunctionExpression( Expression parent ) : base ( parent )
        {
            Arguments = new List<Expression>();
        }

        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }

        public override string Value
        {
            get
            {
                string[] args = Arguments.Select( arg => arg.Value ).ToArray();
                return String.Format( "{0}({1})", Name, String.Join( Constants.Comma, args ) );
            }
        }

        public override bool CanInline
        {
            get
            {
                return Arguments.All( arg => arg.CanInline ) && Value.Length < 40;
            }
        }
    }
}
