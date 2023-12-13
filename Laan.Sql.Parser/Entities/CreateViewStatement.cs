using System;

namespace Laan.Sql.Parser.Entities
{
    public class CreateViewStatement : Statement
    {
        public IStatement Definition { get; set; }
        public bool IsAlter { get; set; }
        public string Name { get; set; }

        public override string Identifier
        {
            get
            {
                return String.Format(
                    "{0} VIEW {1} AS {2}",
                    IsAlter ? "ALTER" : "CREATE",
                    Name,
                    Definition.Identifier
                );
            }
        }
    }
}