using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Laan.SQL.Formatter.Test
{
    public class BaseFormattingTest
    {
        protected static void Compare( string actual, string[] formatted )
        {
            Assert.AreElementsEqual( formatted, actual.Split( new string[] { "\r\n" }, StringSplitOptions.None ) );
        }
    }
}
