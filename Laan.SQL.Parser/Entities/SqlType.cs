using System;

namespace Laan.Sql.Parser.Entities
{
    public class SqlType
    {
        public SqlType(string name)
        {
            Name = name;
            Collation = null;
        }

        public string Name { get; set; }
        public string Collation { get; set; }
        public int? Length { get; set; }
        public bool Max { get; set; }
        public int? Scale { get; set; }

        public override string ToString()
        {
            string lengthDisplay = null;
            if (Length.HasValue)
                lengthDisplay = String.Format(
                    "({0})" + (Collation != null ? " " + Collation : String.Empty),
                    Length
                );
            else if (Max)
                lengthDisplay = "(MAX)";

            string precisionDisplay = null;
            if (Scale.HasValue)
                precisionDisplay = String.Format("({0}, {1})", Length, Scale);

            return String.Format("{0}{1}", Name, precisionDisplay ?? lengthDisplay ?? "");
        }
    }
}
