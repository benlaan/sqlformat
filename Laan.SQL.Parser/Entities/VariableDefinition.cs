using System;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class VariableDefinition
    {
        /// <summary>
        /// Initializes a new instance of the VariableDefinition class.
        /// </summary>
        public VariableDefinition( string name, string type )
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public Expression DefaultValue { get; set; }
    }
}
