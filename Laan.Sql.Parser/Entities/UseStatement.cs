using System;

namespace Laan.Sql.Parser.Entities
{
    public class UseStatement : Statement
    {
        public string DatabaseName { get; set; }

        public override string Identifier
        {
            get { return "USE"; }
        }
    }
}
