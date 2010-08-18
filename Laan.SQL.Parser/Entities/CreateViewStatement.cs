using System;

namespace Laan.Sql.Parser.Entities
{
    public class CreateViewStatement : IStatement
    {
        public bool Terminated { get; set; }
        public SelectStatement SelectBlock { get; set; }
        public string Name { get; set; }

        #region IStatement Members

        public string Identifier
        {
            get { return String.Format( "CREATE VIEW {0} AS {1}", Name, SelectBlock.Identifier ); }
        }

        #endregion
    }
}
