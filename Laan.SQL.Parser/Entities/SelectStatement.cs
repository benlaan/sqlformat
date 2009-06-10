using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{

    public class BaseStatement : IStatement
    {
        /// <summary>
        /// Initializes a new instance of the BaseStatement class.
        /// </summary>
        public BaseStatement()
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

    public class SelectStatement : BaseStatement
    {
        public SelectStatement() : base()
        {
            Distinct = false;
            Top = null;
            OrderBy = new List<Field>();
            GroupBy = new List<Field>();
        }

        public bool Distinct { get; set; }
        public int? Top { get; set; }
        public List<Field> OrderBy { get; set; }
        public List<Field> GroupBy { get; set; }
        public Expression Having { get; set; }
    }
}
