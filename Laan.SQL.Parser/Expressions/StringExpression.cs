using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    public class StringExpression : Expression
    {
        public StringExpression( string value, Expression parent ) : base ( parent )
        {
            Value = value;
        }
    }
}
