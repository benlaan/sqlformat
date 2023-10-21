using System;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class Statement : IStatement
    {
        public bool Terminated { get; set; }

        public virtual string Identifier
        {
            get { return String.Empty; }
        }
    }
}
