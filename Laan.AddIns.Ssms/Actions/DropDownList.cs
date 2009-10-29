using System;
using System.Linq;
using System.Drawing;
using Form = System.Windows.Forms;

using Laan.AddIns.Core;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

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

    public abstract class DropDownList : Laan.AddIns.Core.Action
    {
        private const string EditorWindowClassName = "DockingView";
        private const int LineNumber_Width = 70;
        private const int Border_Width = 4;

        private Font _font;
        private SizeF _fontSize;
        private Form.ListBox _listBox;
        private Window _window;
        private bool _showLineNumbers;

        public DropDownList( AddIn addIn ) : base( addIn )
        {
        }

        private void Initialise()
        {
            _showLineNumbers = ReadProperty<bool>( "TextEditor", "SQL", "ShowLineNumbers" );
            string fontFamily = ReadProperty<string>( "FontsAndColors", "TextEditor", "FontFamily" );
            float fontSize = ReadProperty<short>( "FontsAndColors", "TextEditor", "FontSize" );

            _font = new Font( fontFamily, fontSize );

            _listBox = new Form.ListBox();
            _listBox.DrawMode = Form.DrawMode.OwnerDrawVariable;
            _listBox.Width = 280;
            _listBox.Height = 200;
            _listBox.DrawItem += ListBoxDrawItem;
            _listBox.MeasureItem += ListBoxMeasureItem;
            _listBox.KeyDown += ListBoxKeyDown;
            _listBox.DoubleClick += ListBoxDoubleClick;
            _listBox.LostFocus += ListBoxLostFocus;

            _window = new Window( (IntPtr) _addIn.TextDocument.DTE.MainWindow.HWnd );
            _window.SetParent( _listBox.Handle );

            _fontSize = TextRenderer.MeasureText( "W", _font );
            _fontSize.Width = TextRenderer.MeasureText( "WW", _font ).Width - _fontSize.Width;
            _fontSize.Height -= 1;

            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListMembers", false );
            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListParams", true );
        }

        private T ReadProperty<T>( string category, string page, string property )
        {
            var prop = _addIn.TextDocument.DTE.get_Properties( category, page );
            return (T) prop.Item( property ).Value;
        }

        private void WriteProperty<T>( string category, string page, string property, T value )
        {
            var prop = _addIn.TextDocument.DTE.get_Properties( category, page );
            prop.Item( property ).Value = value;
        }

        private void InternalExecute()
        {
            if ( _listBox.Items.Count == 1 )
            {
                ExecuteItem( _listBox.Items[ 0 ] as Item );
                Done();
            }
            else
            {
                _listBox.Show();
                _listBox.Focus();

                if ( _listBox.Items.Count > 0 )
                    _listBox.SelectedIndex = 0;
            }
        }

        private void Done()
        {
            _listBox.Hide();
            _window.SetFocus();
        }

        #region ListBox Events

        private void ListBoxLostFocus( object sender, EventArgs e )
        {
            Done();
        }

        private void ListBoxDoubleClick( object sender, EventArgs e )
        {
            if ( _listBox.SelectedIndex >= 0 )
                ExecuteItem( _listBox.Items[ _listBox.SelectedIndex ] as Item );
            Done();
        }

        private void ListBoxKeyDown( object sender, Form.KeyEventArgs e )
        {
            Form.ListBox lb = (Form.ListBox) sender;

            if ( e.KeyCode == Form.Keys.Escape || lb.SelectedIndex < 0 )
                Done();
            else 
                if ( e.KeyCode == Form.Keys.Enter )
                {
                    ExecuteItem( (Item) lb.Items[ lb.SelectedIndex ] );
                    _addIn.CancelSelection();
                    Done();
                }
        }

        private void ListBoxMeasureItem( object sender, Form.MeasureItemEventArgs e )
        {
            var item = _listBox.Items[ e.Index ] as Item;
            e.ItemHeight = (int) e.Graphics.MeasureString( item.TightDescription, _font ).Height + 1;
        }

        private void ListBoxDrawItem( object sender, Form.DrawItemEventArgs e )
        {
            if ( e.Index == -1 )
            {
                using ( SolidBrush foreBrush = new SolidBrush( Color.Red ) )
                    e.Graphics.DrawString( "No templates found", _font, foreBrush, new PointF( 50, 30 ) );

                return;
            }
            else
            {
                var selected = ( e.State & System.Windows.Forms.DrawItemState.Selected ) > 0;
                SolidBrush backBrush = new SolidBrush( selected ? Color.CornflowerBlue : e.BackColor );
                e.Graphics.FillRectangle( backBrush, e.Bounds );

                var item = _listBox.Items[ e.Index ] as Item;

                using ( SolidBrush nameBrush = new SolidBrush( selected ? Color.Black : e.ForeColor ) )
                using ( SolidBrush descriptionBrush = new SolidBrush( selected ? e.ForeColor : Color.Blue ) )
                {
                    e.Graphics.DrawString( item.Name, _font, nameBrush, e.Bounds.Left, e.Bounds.Top );

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

        #endregion

        protected abstract void ExecuteItem( Item item );
        protected abstract IEnumerable<Item> GetItems();

        public override bool CanExecute()
        {
            return (
                _addIn.IsCurrentDocumentExtension( "sql" )
                &&
                _addIn.CurrentSelection == ""
                &&
                _addIn.CurrentWord != ""
            );
        }

        public override void Execute()
        {
            if ( _window == null )
                Initialise();

            _listBox.Items.Clear();
            _listBox.Items.AddRange( GetItems().ToArray() );
            _listBox.Height = Math.Min( 290, Math.Max( 100, _listBox.Items.Count * 20 ) );

            var editor = _window.FindByClassName( EditorWindowClassName );
            var point = editor.GetScreenPoint( _window );
            var cursor = _addIn.VirtualCursor;

            int lineNumberWidth = _showLineNumbers ? LineNumber_Width : 0;
            point.X += lineNumberWidth + Border_Width + (int) ( cursor.Column * Math.Ceiling( _fontSize.Width ) );
            point.Y += Border_Width + (int) ( cursor.Row * Math.Ceiling( _fontSize.Height ) );

            _listBox.Location = new Point( point.X, point.Y );
            InternalExecute();
        }
    }
}
