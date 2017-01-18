using System;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class CreateProcedureStatement : Statement
    {
        public CreateProcedureStatement()
        {
            Arguments = new List<VariableDefinition>();
        }

        public string Name { get; set; }
        public bool IsAlter { get; set; }
        public bool IsShortForm { get; set; }
        public bool HasBracketedArguments { get; set; }

        public IStatement Definition { get; set; }
        public List<VariableDefinition> Arguments { get; set; }

        public override string Identifier
        {
            get
            {
                return String.Format(
                    "{0} {1} {2} AS {3}",
                    IsAlter ? "ALTER" : "CREATE",
                    IsShortForm ? "PROC" : "PROCEDURE",
                    Name,
                    Definition.Identifier
                );
            }
        }
    }
}