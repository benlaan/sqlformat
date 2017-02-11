using System;
using System.Linq;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public abstract class BaseCursorAction : Core.BaseAction
    {
        public BaseCursorAction(AddIn addIn) : base(addIn)
        {
        }

        protected static bool IsCapital(string line, int position)
        {
            if (position < 0)
                return true;

            return line[position] >= 'A' && line[position] <= 'Z';
        }

        protected static bool IsSpace(string rightOfCursor, int position)
        {
            if (position > rightOfCursor.Length)
                return false;

            return rightOfCursor[position] == ' ';
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql")
                && AddIn.AllText.Length > 0;
        }
    }
}