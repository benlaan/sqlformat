using System;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class DeclareStatement : Statement
    {
        public DeclareStatement()
        {
            Definitions = new List<VariableDefinition>();
        }

        public List<VariableDefinition> Definitions { get; set; }
    }
}
