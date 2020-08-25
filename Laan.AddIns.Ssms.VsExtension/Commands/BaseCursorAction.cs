using System;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension
{
    public abstract class BaseCursorAction : BaseAction
    {
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

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && AllText.Length > 0;
        }
    }
}
