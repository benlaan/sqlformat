using System;
using System.Collections.Generic;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class ProjectionStatement : CustomStatement
    {
        /// <summary>
        /// Initializes a new instance of the BaseStatement class.
        /// </summary>
        public ProjectionStatement()
        {
            Fields = new List<Field>();
        }

        public List<Field> Fields { get; set; }
    }
}
