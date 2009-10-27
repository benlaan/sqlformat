using System;

namespace Laan.SQL.Parser
{
    public class CreateViewStatement : IStatement
    {
        public bool Terminated { get; set; }
        public SelectStatement SelectBlock { get; set; }
        public string Name { get; set; }

        #region IStatement Members

        public string Value
        {
            get { return "CREATE VIEW " + Name + "AS " + SelectBlock.Value; }
        }

        #endregion
    }
}
