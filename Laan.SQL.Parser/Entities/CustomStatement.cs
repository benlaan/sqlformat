using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class CustomStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the BaseStatement class.
        /// </summary>
        public CustomStatement()
        {
            From = new List<Table>();
            Joins = new List<Join>();
        }

        public List<Join> Joins { get; set; }
        public List<Table> From { get; set; }
        public Expression Where { get; set; }
    }
}