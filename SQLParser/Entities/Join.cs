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

    public class Join : AliasedEntity
    {
        public Join()
        {
            Condition = new CriteriaExpression();
        }

        public string Name { get; set; }
        public JoinType Type { get; set; }
        public Expression Condition { get; set; }

        public override string Value
        {
            get { return Name + Alias.Value + " ON " + Condition.Value; }
            protected set { base.Value = value; }
        }
    }
}
