using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class CustomStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the BaseStatement class.
        /// </summary>
        public CustomStatement()
        {
            From = new List<Table>();
        }

        public List<Table> From { get; set; }
        public Expression Where { get; set; }

    }
}