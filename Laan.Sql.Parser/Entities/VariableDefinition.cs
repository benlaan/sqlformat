using System;
using System.Diagnostics;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    [DebuggerDisplay("{Name}: {Type} = {DefaultValue}")]
    public class VariableDefinition
    {
        /// <summary>
        /// Initializes a new instance of the VariableDefinition class.
        /// </summary>
        public VariableDefinition(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public Expression DefaultValue { get; set; }
    }
}
