using System;
using EnvDTE;

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

    }
}
