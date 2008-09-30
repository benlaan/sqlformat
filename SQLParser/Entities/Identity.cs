using System;

namespace Laan.SQL.Parser
{
    public class Identity
    {
        public int Start { get; set; }
        public int Increment { get; set; }

        public override int GetHashCode()
        {
            return Start.GetHashCode() + Increment.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            Identity other = (Identity) obj;
            return Start == other.Start && Increment == other.Increment;
        }

        public override string ToString()
        {
            return String.Format( "IDENTITY({0}{1}{2})", Start, ",", Increment );
        }
    }
}
