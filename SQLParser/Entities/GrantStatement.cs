using System;
using System.Collections.Generic;

namespace Laan.SQL.Parser
{
    public class GrantStatement : IStatement
    {
        public GrantStatement()
        {

        }

        public string Operation { get; set; }
        public string TableName { get; set; }
        public string Grantee { get; set; }
    }
}
