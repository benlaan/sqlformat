using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    public class NestedExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the NestedExpression class.
        /// </summary>
        public NestedExpression( Expression parent ) : base ( parent )
        {
        }

        public Expression Expression { get; set; }

        public override string Value
        {
            get { return Constants.OpenBracket + Expression.Value + Constants.CloseBracket; }
        }
    }
}
