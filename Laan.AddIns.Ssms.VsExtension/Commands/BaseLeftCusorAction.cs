using System;
using System.Linq;

using Laan.AddIns.Ssms.VsExtension.Utils;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public abstract class BaseLeftCusorAction : BaseCursorAction
    {
        protected void CursorLeft(bool applySelection)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var textDocument = TextDocument;
            var cursor = new Cursor(textDocument.Selection.CurrentColumn, textDocument.Selection.TopPoint.Line);

            if (cursor.Column == 1)
                return;

            var line = CurrentLine;
            if (String.IsNullOrEmpty(line))
                return;

            var leftOfCursor = line.Substring(0, cursor.Column - 1);

            var position = leftOfCursor.Length - 1;
            while (position > 0)
            {
                if (IsSpace(leftOfCursor, position) && position != leftOfCursor.Length - 1)
                {
                    textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length - position);
                    return;
                }

                if (IsCapital(leftOfCursor, position) && !IsCapital(leftOfCursor, position - 1))
                {
                    textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length - position);

                    return;
                }

                position--;
            }

            textDocument.Selection.CharLeft(applySelection, leftOfCursor.Length);
        }
    }
}