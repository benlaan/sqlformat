using System;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class SetStatement : Statement
    {
        public SetStatement()
        {

        }

        public Expression Expression { get; set; }
        public string Variable { get; set; }
    }
}
