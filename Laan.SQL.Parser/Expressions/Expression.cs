using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Laan.SQL.Parser.Expressions
{
    [DebuggerDisplay( "{Value}" )]
    public class Expression : IInlineFormattable
    {
        /// <summary>
        /// Initializes a new instance of the Expression class.
        /// </summary>
        public Expression( Expression parent )
        {
            Parent = parent;
        }

        public virtual string Value { get; protected set; }
        public Expression Parent { get; set; }

        #region IInlineFormattable Members

        public virtual bool CanInline
        {
            get { return false; }
        }

        #endregion
    }
}