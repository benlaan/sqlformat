using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace Laan.SQL.Parser
{
    public enum JoinType
    {
        [Description( "JOIN" )]
        Join,

        [Description( "INNER JOIN" )]
        InnerJoin,

        [Description( "LEFT JOIN" )]
        LeftJoin,

        [Description( "LEFT OUTER JOIN" )]
        LeftOuterJoin,

        [Description( "RIGHT JOIN" )]
        RightJoin,

        [Description( "RIGHT OUTER JOIN" )]
        RightOuterJoin,

        [Description( "FULL JOIN" )]
        FullJoin,

        [Description( "FULL OUTER JOIN" )]
        FullOuterJoin,
        
        [Description( "CROSS JOIN" )]
        CrossJoin
    }

    public class Join : AliasedEntity
    {
        public Join()
        {
            Condition = new CriteriaExpression();

            _joinMap = new Dictionary<JoinType, string> 
            { 
                { JoinType.Join, "JOIN" },
                { JoinType.InnerJoin, "INNER JOIN" },
                { JoinType.LeftJoin, "LEFT JOIN" },
                { JoinType.LeftOuterJoin, "LEFT OUTER JOIN" },
                { JoinType.RightJoin, "RIGHT JOIN" },
                { JoinType.RightOuterJoin, "RIGHT OUTER JOIN" },
                { JoinType.FullJoin, "FULL JOIN" },
                { JoinType.FullOuterJoin, "FULL OUTER JOIN" },
                { JoinType.CrossJoin, "CROSS JOIN" }
            };
        }

        protected Dictionary<JoinType, string> _joinMap;

        public string Name { get; set; }
        public JoinType Type { get; set; }
        public Expression Condition { get; set; }

        public override string Value
        {
            get { return String.Format("{0} {1}{2}", _joinMap[ Type ], Name, Alias.Value); }
            protected set { base.Value = value; }
        }

        public int Length { get { return _joinMap[ Type ].Length; } }
    }

    public class DerivedJoin : Join
    {
        public SelectStatement SelectStatement { get; set; }

        public override string Value
        {
            get { return String.Format("{0} (", _joinMap[ Type ]); }
            protected set { base.Value = value; }
        }
    }
}