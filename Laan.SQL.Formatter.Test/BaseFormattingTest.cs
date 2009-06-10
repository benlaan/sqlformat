using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Laan.SQL.Formatter.Test
{
    public class BaseFormattingTest
    {
        private static string DisplayLists( string[] expected, string[] actual )
        {
            const int width = 60;
            string LineFormat = String.Format( "{{0,-{0}}} | {{1,-{0}}}\n", width );

            StringBuilder result = new StringBuilder();
            result.AppendFormat( LineFormat, "Expected", "Actual" );
            result.AppendFormat( "{0}\n", new string( '-', width * 2 + 3 ) );

            for ( int index = 0; index < Math.Max( expected.Length, actual.Length ); index++ )
            {
                result.AppendFormat( 
                    LineFormat, 
                    GetLine( index, expected ), 
                    GetLine( index, actual ) 
                );
            }

            return result.ToString();
        }

        private static string GetLine( int index, string[] lines )
        {
            return index < lines.Length ? lines[ index ] : "";
        }

        protected static void Compare( string actual, string[] expected )
        {
            var actualAsList = actual.Split( new string[] { "\r\n" }, StringSplitOptions.None );
            Assert.AreElementsEqual( expected, actualAsList, DisplayLists( expected, actualAsList ) );
        }
    }
}
