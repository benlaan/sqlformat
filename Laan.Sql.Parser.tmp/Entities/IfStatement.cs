using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class IfStatement : CustomStatement
    {
        public IStatement If { get; set; }
        public Expression Condition { get; set; }
        public IStatement Else { get; set; }
    }
}
