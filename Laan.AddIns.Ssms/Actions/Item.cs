using System;

namespace Laan.AddIns.Ssms.Actions
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string TightDescription
        {
            get
            {
                return String.Join(
                    "\n",
                    Description.Split(
                        new []{"\n"},
                        StringSplitOptions.RemoveEmptyEntries
                        )
                    );

            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}