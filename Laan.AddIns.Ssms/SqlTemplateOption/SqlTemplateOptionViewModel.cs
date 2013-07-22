using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Laan.AddIns.Forms;
using System.Collections;

namespace Laan.AddIns.Ssms.Actions
{
    public class SqlTemplateOptionViewModel : INotifyPropertyChanged, IDialog
    {
        private bool _isDirty;
        private Template _selectedTemplate;

        /// <summary>
        /// Initializes a new instance of the SqlTemplateOptionViewModel class.
        /// </summary>
        /// <param name="templates"></param>
        public SqlTemplateOptionViewModel(List<Template> templates)
        {
            Templates = new ObservableCollection<Template>(templates);

            Save = new DelegateCommand(ExecuteSave, IsDirty);
            Cancel = new DelegateCommand(ExecuteCancel);
            Add = new DelegateCommand(ExecuteAdd);
            Remove = new DelegateCommand(ExecuteRemove, () => SelectedTemplate != null);

            Templates.CollectionChanged += CollectionChanged;
            AssignPropertyChangedHandler(Templates);
            SelectedTemplate = Templates.FirstOrDefault();
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs ev)
        {
            MarkAsDirty();
        }

        private void AssignPropertyChangedHandler(IList newItems)
        {
            foreach (var item in newItems.OfType<INotifyPropertyChanged>())
                item.PropertyChanged += ItemPropertyChanged;
        }

        private void CollectionChanged(object s, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Remove.RaiseCanExecuteChanged();

            AssignPropertyChangedHandler(e.NewItems);
            foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                item.PropertyChanged -= ItemPropertyChanged;
        }

        private bool IsDirty()
        {
            return _isDirty;
        }

        private void ExecuteSave()
        {
            _isDirty = false;
            if (OnSave != null)
                OnSave(this, EventArgs.Empty);
        }

        private void ExecuteCancel()
        {
            if (_isDirty)
            {
                string confirmCancel = "There are unsaved changes - are you sure you wish to cancel?";

                if (MessageBox.Show(confirmCancel, "Confirm Cancel changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                _isDirty = false;
            }

            if (OnCancel != null)
                OnCancel(this, EventArgs.Empty);
        }

        private void ExecuteAdd()
        {
            Template newTemplate = new Template() { Name = "New Template" };
            Templates.Add(newTemplate);
            SelectedTemplate = newTemplate;
            MarkAsDirty();
        }

        private void ExecuteRemove()
        {
            var indexOfRemovedItem = Templates.IndexOf(SelectedTemplate);
            if (indexOfRemovedItem >= 0)
                Templates.RemoveAt(indexOfRemovedItem);

            if (Templates.Count > 0)
                SelectedTemplate = Templates[Clamp(indexOfRemovedItem, 0, Templates.Count - 1)];

            MarkAsDirty();
        }

        private void MarkAsDirty()
        {
            _isDirty = true;
            Save.RaiseCanExecuteChanged();
        }

        private int Clamp(int item, int minimum, int maximum)
        {
            if (item < minimum)
                return minimum;

            if (item > maximum)
                return maximum;

            return item;
        }

        public ObservableCollection<Template> Templates { get; set; }

        public Template SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate == value)
                    return;

                _selectedTemplate = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedTemplate"));

                Remove.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnSave;
        public event EventHandler OnCancel;

        public DelegateCommand Save { get; set; }
        public DelegateCommand Cancel { get; set; }
        public DelegateCommand Add { get; set; }
        public DelegateCommand Remove { get; set; }

        public bool CanCancel
        {
            get { return !_isDirty; }
        }
    }
}
