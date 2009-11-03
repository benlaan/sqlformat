using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Expressions
{
    public class SelectExpression : Expression
    {

        public SelectExpression() : base( null )
        {
            Statement = new SelectStatement();
        }

        public SelectStatement Statement { get; set; }

        public override string Value
        {
            get { return Statement.ToString(); }
        }
    }
}
