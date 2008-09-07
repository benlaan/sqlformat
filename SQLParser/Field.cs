using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLParser
{
    public class Field
    {
        public Field()
        {
        }

        public string Name { get; set; }
        public string? Alias { get; set; }
    }
}
