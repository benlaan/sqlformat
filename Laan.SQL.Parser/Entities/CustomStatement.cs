using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class CustomStatement : IStatement
    {
        /// <summary>
        /// Initializes a new instance of the BaseStatement class.
        /// </summary>
        public CustomStatement()
        {
            Fields = new List<Field>();
            From = new List<Table>();
            Joins = new List<Join>();
        }

        public List<Field> Fields { get; set; }
        public List<Join> Joins { get; set; }
        public List<Table> From { get; set; }
        public Expression Where { get; set; }
    }
}
