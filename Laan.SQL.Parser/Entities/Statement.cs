using System;
using System.Collections.Generic;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class Statement : IStatement
    {
        public bool Terminated { get; set; }

        #region IStatement Members

        public virtual string Identifier
        {
            get { return ""; }
        }

        #endregion
    }
}
