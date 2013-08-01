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

    public class ExecStatement : Statement
    {
        public ExecStatement()
        {
            Arguments = new List<Argument>();
        }

        public string FunctionName { get; set; }
        public List<Argument> Arguments { get; private set; }
    }

    public class ExecuteSqlStatement : ExecStatement
    {
        public ExecuteSqlStatement()
        {
        }

        public IStatement InnerStatement { get; set; }
    }
}
