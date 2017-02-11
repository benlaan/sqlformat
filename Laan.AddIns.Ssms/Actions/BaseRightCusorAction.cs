using System;
using System.Linq;

using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public abstract class BaseRightCusorAction : BaseCursorAction
    {
        public BaseRightCusorAction(AddIn addIn) : base(addIn)
        {
        }

        protected void CursorRight(bool applySelection)
        {
            var textDocument = AddIn.TextDocument;
            var cursor = new Cursor(textDocument.Selection.CurrentColumn, textDocument.Selection.TopPoint.Line);

            if (cursor.Column == AddIn.CurrentLine.Length)
                return;

            var line = AddIn.CurrentLine;
            if (String.IsNullOrEmpty(line))
                return;

            var rightOfCursor = line.Substring(cursor.Column - 1, line.Length - cursor.Column + 1);

            var position = 1;
            while (position < rightOfCursor.Length - 1)
            {
                if (IsSpace(rightOfCursor, position))
                {
                    textDocument.Selection.CharRight(applySelection, position + 1);
                    return;
                }

                if (IsCapital(rightOfCursor, position)
                    && !IsCapital(rightOfCursor, position + 1)
                    && !IsSpace(rightOfCursor, position + 1)
                )
                {
                    textDocument.Selection.CharRight(applySelection, position);
                    return;
                }

                position++;
            }

            textDocument.Selection.CharRight(applySelection, rightOfCursor.Length);
        }
    }
}