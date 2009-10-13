using System;
using System.Collections.Generic;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class GrantStatement : Statement
    {
        public GrantStatement()
        {

        }

        public string Operation { get; set; }
        public string TableName { get; set; }
        public string Grantee { get; set; }
    }
}