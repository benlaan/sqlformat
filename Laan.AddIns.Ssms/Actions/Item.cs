using System;

namespace Laan.AddIns.Ssms.Actions
{
    public class Item
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string TightDescription
        {
            get
            {
                return String.Join(
                    "\n",
                    Name.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                );
            }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}