using System.Collections.Generic;
using System;

namespace Laan.SQL.Parser
{
    public class SelectStatement : IStatement
    {
        public SelectStatement()
        {
            Distinct = false;
            Top = null;
            Fields = new List<Field>();
            From = new List<Table>();
            Joins = new List<Join>();
            OrderBy = new List<Field>();
            GroupBy = new List<Field>();
        }

        public bool Distinct { get; set; }
        public int? Top { get; set; }

        public List<Table> From { get; set; }
        public List<Field> Fields { get; set; }
        public List<Join> Joins { get; set; }
        public Expression Where { get; set; }
        public List<Field> OrderBy { get; set; }
        public List<Field> GroupBy { get; set; }
        public Expression Having { get; set; }
    }
}
