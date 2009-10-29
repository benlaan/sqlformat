using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class BlockStatement : CustomStatement
    {
        /// <summary>
        /// Initializes a new instance of the BlockStatement class.
        /// </summary>
        public BlockStatement()
        {
            Statements = new List<IStatement>();
        }

        public List<IStatement> Statements { get; set; }
    }
}
