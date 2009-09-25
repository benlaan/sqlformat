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
            From = new List<Table>();
            Joins = new List<Join>();
            Terminated = false;
        }

        public List<Join> Joins { get; set; }
        public List<Table> From { get; set; }
        public Expression Where { get; set; }
        public bool Terminated { get; set; }
    }
}