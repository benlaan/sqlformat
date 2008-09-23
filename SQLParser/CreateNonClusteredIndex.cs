using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.SQL.Parser
{
    public class CreateNonClusteredIndex : IStatement
    {
        public string TableName { get; set; }
        public string ConstraintName { get; set; }
        public string KeyName { get; set; }
    }
}
