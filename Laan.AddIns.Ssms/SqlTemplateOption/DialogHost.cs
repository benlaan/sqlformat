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
    }

    public partial class DialogHost : Form
    {
        public DialogHost()
        {
            InitializeComponent();
        }

        public DialogResult Show<T>(object model) where T : FrameworkElement, new()
        {
            var element = new T() { DataContext = model };
            wpfElementHost.AutoSize = true;
            this.Width = (int)element.MinWidth + 50;
            this.Height = (int)element.MinHeight + 50;
            wpfElementHost.HostContainer.Children.Add(element);

            IDialog dialog = model as IDialog;
            if (dialog != null)
            {
                dialog.OnSave += dialog_Save;
                dialog.OnCancel += dialog_Cancel;
            }

            return ShowDialog();
        }

        void dialog_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void dialog_Save(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
