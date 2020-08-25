using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Laan.AddIns.Forms
{
    public interface IDialog
    {
        event EventHandler OnSave;
        event EventHandler OnCancel;
        bool CanCancel { get; }
   }

    public partial class DialogHost : Form
    {
        private IDialog _dialog;

        public DialogHost()
        {
            InitializeComponent();
        }

        public DialogResult Show<T>(object model) where T : FrameworkElement, new()
        {
            var element = new T() { DataContext = model };
            
            Height = (int)element.Height;
            Width = (int)element.Width;

            wpfElementHost.AutoSize = true;
            wpfElementHost.Child = element;

            KeyPreview = true;

            _dialog = (IDialog)model;
            if (_dialog != null)
            {
                _dialog.OnSave += dialog_Save;
                _dialog.OnCancel += dialog_Cancel;
            }

            KeyDown += (sender, e) => 
            {
                if (CanCancel())
                    CancelDialog();
            };
            this.FormClosing += DialogHost_FormClosing;

            return ShowDialog();
        }

        private void DialogHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CanCancel();
        }

        private void dialog_Cancel(object sender, EventArgs e)
        {
            CancelDialog();
        }

        private void dialog_Save(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool CanCancel()
        {
            return _dialog == null || _dialog.CanCancel;
        }

        private void CancelDialog()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
