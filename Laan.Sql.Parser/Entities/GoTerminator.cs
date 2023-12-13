using System;

namespace Laan.Sql.Parser.Entities
{
    public class GoTerminator : IStatement
    {

        #region IStatement Members

        public bool Terminated { get; set; }
        public string Identifier
        {
            get { return "GO"; }
        }

        #endregion
    }
}
