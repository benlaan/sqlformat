using System;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public abstract class SetStatement : Statement
    {
        public SetStatement()
        {

        }

        public Expression Assignment { get; set; }
    }
}