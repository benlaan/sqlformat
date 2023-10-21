using System;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class SetOptionStatement : SetStatement
    {
        public SetOptionStatement()
        {

        }

        public string Option { get; set; }
    }
}
