using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression( string value, Expression parent ) : base( parent )
        {
            Parts = value.Split( new string[] { Constants.Dot }, StringSplitOptions.RemoveEmptyEntries ).ToList();
        }

        public List<string> Parts { get; set; }

        public override string Value
        {
            get { return String.Join( Constants.Dot, Parts.ToArray() ); }
        }
    }
}
