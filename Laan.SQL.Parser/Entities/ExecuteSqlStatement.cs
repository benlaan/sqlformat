using System;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class Argument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }

    public class ExecuteSqlStatement : Statement
    {
        public ExecuteSqlStatement()
        {
            Arguments = new List<Argument>();
        }

        public string FunctionName { get; set; }
        public List<Argument> Arguments { get; private set; }
        public IStatement InnerStatement { get; set; }
    }
}
