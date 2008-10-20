using System.Collections.Generic;
using System;

namespace Laan.SQL.Parser
{
    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin
    }

    public class Join
    {
        public Join()
        {
            Condition = new CriteriaExpression();
        }

        public string Name { get; set; }
        public string Alias { get; set; }
        public JoinType Type { get; set; }
        public Expression Condition { get; set; }
    }
}
