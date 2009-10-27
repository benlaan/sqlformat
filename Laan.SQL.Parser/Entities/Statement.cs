using System;
using System.Collections.Generic;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class Statement : IStatement
    {
        public bool Terminated { get; set; }

        #region IStatement Members

        public virtual string Value
        {
            get { return ""; }
        }

        #endregion
    }
}
