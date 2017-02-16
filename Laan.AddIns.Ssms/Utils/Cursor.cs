using System;
using System.Diagnostics;

namespace Laan.AddIns.Core
{
    [DebuggerDisplay("Col: {Column} | Row: {Row}")]
    public class Cursor
    {
        public Cursor(int col, int row)
        {
            Column = col;
            Row = row;
        }

        public int Column { get; set; }
        public int Row { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Cursor;
            if (other == null)
                return false;

            return other.Row == Row && other.Column == Column;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() 
                 ^ Column.GetHashCode();
        }
    }
}
