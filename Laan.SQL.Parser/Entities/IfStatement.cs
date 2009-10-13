using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class IfStatement : Statement
    {
        public IStatement If { get; set; }
        public Expression Condition { get; set; }
        public IStatement Else { get; set; }
    }
}
