using System;

namespace Laan.AddIns.Core
{
    public class Cursor
    {
        public Cursor( int col, int row )
        {
            Column = col;
            Row = row;
        }

        public int Column { get; set; }
        public int Row { get; set; }
    }
}
