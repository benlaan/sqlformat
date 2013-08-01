using System;
using System.Linq;
using System.Drawing;
using Form = System.Windows.Forms;

using Laan.AddIns.Core;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Laan.AddIns.Ssms.Actions
{
    public abstract class DropDownList : Core.Action
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
            string fontFamily = ReadProperty<string>( "FontsAndColors", "TextEditor", "FontFamily", "Courier New" );
            short fontSize = ReadProperty<short>( "FontsAndColors", "TextEditor", "FontSize", 10 );

            _font = new Font( fontFamily, ( float )fontSize );

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

            _window = new Window( (IntPtr) AddIn.TextDocument.DTE.MainWindow.HWnd );

            _fontSize = TextRenderer.MeasureText( "W", _font );
            _fontSize.Width = TextRenderer.MeasureText( "WW", _font ).Width - _fontSize.Width;
            _fontSize.Height -= 1;

            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListMembers", false );
            //WriteProperty<bool>( "TextEditor", "SQL", "AutoListParams", true );
            _window.SetParent( _listBox.Handle );
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
                    AddIn.CancelSelection();
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
                    e.Graphics.DrawString( item.Code, _font, nameBrush, e.Bounds.Left, e.Bounds.Top );

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
                AddIn.IsCurrentDocumentExtension( "sql" )
                &&
                AddIn.CurrentSelection == ""
                &&
                AddIn.CurrentWord != ""
            );
        }

        public override void Execute()
        {
            if ( _window == null )
                Initialise();

            if (PageExists( "TextEditor", "SQL" ))
                _showLineNumbers = ReadProperty<bool>( "TextEditor", "SQL", "ShowLineNumbers", false );
            else
                _showLineNumbers = ReadProperty<bool>( "TextEditor", "AllLanguages", "ShowLineNumbers", false );

            _listBox.Items.Clear();
            _listBox.Items.AddRange( GetItems().ToArray() );
            _listBox.Height = Math.Min( 300, Math.Max( 150, _listBox.Items.Count * 20 ) );

            var editor = _window.FindByClassName( EditorWindowClassName );
            var point = editor.GetScreenPoint( _window );
            var cursor = AddIn.VirtualCursor;

            int lineNumberWidth = _showLineNumbers ? LineNumber_Width : 25;
            point.X += lineNumberWidth + Border_Width + (int) ( cursor.Column * Math.Ceiling( _fontSize.Width ) );
            point.Y += Border_Width + (int) ( cursor.Row * Math.Ceiling( _fontSize.Height ) );

            _listBox.Location = new Point( point.X, point.Y );
            InternalExecute();
        }
    }
}
