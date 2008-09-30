using System;

namespace Laan.SQL.Parser
{
    public class SqlType
    {
        public SqlType( string name, int length, int scale ) : this( name, length )
        {
            Scale = scale;
        }

        public SqlType( string name, int length ) : this( name )
        {
            Length = length;
        }

        public SqlType( string name )
        {
            Name = name;
            Collation = null;
        }

        public string Name { get; set; }
        public string Collation { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public override string ToString()
        {
            string lengthDisplay = null;
            if ( Length.HasValue )
                lengthDisplay = String.Format( 
                    "({0})" + 
                    ( Collation != null ? " " + Collation : "" ), 
                    Length 
                );

            string precisionDisplay = null;
            if ( Scale.HasValue )
                precisionDisplay = String.Format( "({0}, {1})", Length, Scale );

            return String.Format( "{0}{1}", Name, precisionDisplay ?? lengthDisplay ?? "" );
        }
    }
}
