using System;

namespace Laan.Sql.Parser.Entities
{
    public class CreateViewStatement : Statement
    {
        public IStatement ScriptBlock { get; set; }
        public string Name { get; set; }

        public override string Identifier
        {
            get { return String.Format( "CREATE VIEW {0} AS {1}", Name, ScriptBlock.Identifier ); }
        }
    }
}
