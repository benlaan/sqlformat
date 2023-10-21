using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
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