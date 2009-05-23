using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Laan.SQL.Parser
{

    public class CriteriaExpression : Expression
    {
        public CriteriaExpression() 
        {
            Left = new Expression();
            Right = new Expression();

        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public string Operator { get; set; }

        public override string Value
        {
            get { return String.Format( "{0} {1} {2}", Left.Value, Operator, Right.Value ); }
        }
    }
}
