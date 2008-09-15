using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SQLParser
{
    [DebuggerDisplay( "{Name}: {Type} {Nullability}" )]
    public class FieldDefinition
    {
        public Nullability Nullability { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
