using System;

namespace Laan.Sql.Parser.Entities
{
    public class GoTerminator : IStatement
    {

        #region IStatement Members

        public bool Terminated { get; set; }
        public string Value
        {
            get { return "GO"; }
        }

        #endregion
    }
}
