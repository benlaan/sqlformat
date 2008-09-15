using System.Collections.Generic;
using System;

namespace Laan.SQLParser
{
    public interface IStatement
    {

    }

    public class SelectStatement : IStatement
    {
        public SelectStatement()
        {
            Distinct = false;
            Top = null;
            Fields = new List<Field>();
            From = new List<Table>();
        }

        public bool Distinct { get; set; }
        public int? Top { get; set; }

        public List<Table> From { get; set; }
        public List<Field> Fields { get; set; }
    }
}
