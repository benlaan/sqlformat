using System;
using EnvDTE;
using Laan.AddIns.Ssms.Actions;

namespace Laan.AddIns.Core
{
    public abstract class Action
    {
        protected readonly AddIn _addIn;

        /// <summary>
        /// Initializes a new instance of the Action class.
        /// </summary>
        public Action( AddIn addIn )
        {
            _addIn = addIn;
        }

        public object ImageIndex { get; set; }

        /// <summary>
        /// Globally Unique Name
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Text for Command Button
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// Hint for command when mouse hovers
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Visual Studio binding
        /// </summary>
        /// <example>
        /// Text Editor::Ctrl+Shift+/
        /// </example>
        public string KeyboardBinding { get; set; }

        public string DisplayName { get; set; }
        public string DescriptivePhrase { get; set; }

        public abstract void Execute();
        public abstract bool CanExecute();

        public string FullName
        {
            get { return String.Format( "Laan.AddIns.Core.Addin.{0}", KeyName ); }
        }

        protected bool PageExists(string category, string page)
        {
            try
            {
                _addIn.TextDocument.DTE.get_Properties( category, page );
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected T ReadProperty<T>( string category, string page, string property, T defaultValue )
        {
            try
            {
                var prop = _addIn.TextDocument.DTE.get_Properties( category, page );
                return ( T )prop.Item( property ).Value;
            }
            catch ( Exception ex )
            {
                _addIn.Error( String.Format("Failed to ReadProperty('{0}', '{1}', '{2}')", category, page, property), ex );
                return defaultValue;
            }
        }

        protected void WriteProperty<T>( string category, string page, string property, T value )
        {
            try
            {
                var prop = _addIn.TextDocument.DTE.get_Properties( category, page );
                prop.Item( property ).Value = value;
            }
            catch ( Exception ex )
            {
                _addIn.Error( String.Format("Failed to WriteProperty('{0}', '{1}', '{2}', '{3})", category, page, property), ex );
            }
        }

    }
}
