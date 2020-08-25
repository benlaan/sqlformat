using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Laan.AddIns.Ssms.VsExtension.Utils;

using Microsoft.VisualStudio.Shell;

using Form = System.Windows.Forms;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public abstract class BaseDropDownAction : BaseAction
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

        private const int LineNumber_Width = 70;
        private const int Border_Width = 4;

        private Font _font;
        private SizeF _fontSize;
        private Form.ListBox _listBox;
        private Window _window;
        private bool _showLineNumbers;

        private void Initialise()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string fontFamily = ReadProperty<string>("FontsAndColors", "TextEditor", "FontFamily", "Courier New");
            short fontSize = ReadProperty<short>("FontsAndColors", "TextEditor", "FontSize", 10);

            _font = new Font(fontFamily, (float)fontSize);

            _listBox = new Form.ListBox();
            _listBox.DrawMode = Form.DrawMode.OwnerDrawVariable;
            _listBox.Width = 340;
            _listBox.Height = 200;
            _listBox.DrawItem += ListBoxDrawItem;
            _listBox.MeasureItem += ListBoxMeasureItem;
            _listBox.KeyDown += ListBoxKeyDown;
            _listBox.DoubleClick += ListBoxDoubleClick;
            _listBox.LostFocus += ListBoxLostFocus;
            _listBox.Hide();

            _window = new Window((IntPtr)TextDocument.DTE.ActiveDocument.ActiveWindow.HWnd);
            _window.DumpWindows();

            _fontSize = TextRenderer.MeasureText("W", _font);
            _fontSize.Width = TextRenderer.MeasureText("WW", _font).Width - _fontSize.Width;
            _fontSize.Height -= 1;

            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListMembers", false );
            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListParams", true );
            _window.SetParent(_listBox.Handle);
        }

        private void InternalExecute()
        {
            if (_listBox.Items.Count == 1)
            {
                ExecuteItem(_listBox.Items[0] as Item);
                Done();
            }
            else
            {
                _listBox.Show();
                _listBox.Focus();

                if (_listBox.Items.Count > 0)
                    _listBox.SelectedIndex = 0;
            }
        }

        private void Done()
        {
            _listBox.Hide();
            _window.SetFocus();
        }

        private void ListBoxLostFocus(object sender, EventArgs e)
        {
            Done();
        }

        private void ListBoxDoubleClick(object sender, EventArgs e)
        {
            if (_listBox.SelectedIndex >= 0)
                ExecuteItem(_listBox.Items[_listBox.SelectedIndex] as Item);
            Done();
        }

        private void ListBoxKeyDown(object sender, Form.KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var lb = (ListBox)sender;

            if (e.KeyCode == Form.Keys.Escape || lb.SelectedIndex < 0)
                Done();
            else
                if (e.KeyCode == Form.Keys.Enter)
                {
                    ExecuteItem((Item)lb.Items[lb.SelectedIndex]);
                    CancelSelection();
                    Done();
                }
        }

        private void ListBoxMeasureItem(object sender, Form.MeasureItemEventArgs e)
        {
            var item = _listBox.Items[e.Index] as Item;
            e.ItemHeight = (int)e.Graphics.MeasureString(item.TightDescription, _font).Height + 1;
        }

        private void ListBoxDrawItem(object sender, Form.DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                using (SolidBrush foreBrush = new SolidBrush(Color.Red))
                    e.Graphics.DrawString("No templates found", _font, foreBrush, new PointF(50, 30));

                return;
            }
            else
            {
                var selected = (e.State & Form.DrawItemState.Selected) > 0;

                using (SolidBrush backBrush = new SolidBrush(selected ? Color.CornflowerBlue : e.BackColor))
                    e.Graphics.FillRectangle(backBrush, e.Bounds);

                var item = _listBox.Items[e.Index] as Item;

                using (SolidBrush nameBrush = new SolidBrush(selected ? Color.Black : e.ForeColor))
                using (SolidBrush descriptionBrush = new SolidBrush(selected ? e.ForeColor : Color.Blue))
                {
                    e.Graphics.DrawString(item.Code, _font, nameBrush, e.Bounds.Left, e.Bounds.Top);

                    e.Graphics.DrawString(
                        item.TightDescription,
                        _font,
                        descriptionBrush,
                        e.Bounds.Left + 50,
                        e.Bounds.Top
                    );
                }
            }
        }

        protected abstract void ExecuteItem(Item item);
        protected abstract IEnumerable<Item> GetItems();

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && CurrentSelection == String.Empty
                && CurrentWord != String.Empty;
        }

        protected override void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _window = null;
            if (_listBox != null)
                _listBox.Dispose();
            _listBox = null;

            Initialise();

            if (PageExists("TextEditor", "SQL"))
                _showLineNumbers = ReadProperty<bool>("TextEditor", "SQL", "ShowLineNumbers", false);
            else
                _showLineNumbers = ReadProperty<bool>("TextEditor", "AllLanguages", "ShowLineNumbers", false);

            _listBox.Items.Clear();
            _listBox.Items.AddRange(GetItems().ToArray());
            _listBox.Height = Math.Min(300, Math.Max(150, _listBox.Items.Count * 20));

            var point = new Point(0, 0);
            var cursor = VirtualCursor;

            int lineNumberWidth = _showLineNumbers ? LineNumber_Width : 25;
            point.X += lineNumberWidth + Border_Width + (int)(cursor.Column * Math.Ceiling(_fontSize.Width));
            point.Y += Border_Width + (int)(cursor.Row * Math.Ceiling(_fontSize.Height + 2));

            _listBox.Location = new Point(point.X, point.Y);
            InternalExecute();
        }
    }
}
