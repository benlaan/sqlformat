using System;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class SetVariableStatement : SetStatement
    {
        public SetVariableStatement()
        {

        }

        public string Variable { get; set; }
    }
}
