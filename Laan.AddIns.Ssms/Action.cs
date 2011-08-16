using System;
using EnvDTE;
using Laan.AddIns.Ssms.Actions;
using Microsoft.Win32;

namespace Laan.AddIns.Core
{
    public abstract class Action
    {
        protected internal AddIn AddIn { get; set; }

        /// <summary>
        /// Initializes a new instance of the Action class.
        /// </summary>
        public Action( AddIn addIn )
        {
            AddIn = addIn;
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
                AddIn.TextDocument.DTE.get_Properties( category, page );
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Because SMSS doesn't appear to allow custom properties to be defined, use Registry instead
        protected T ReadConfigValue<T>( string name, T defaultValue )
        {
            using ( var laanSoftwareKey = Registry.CurrentUser.CreateSubKey( "Laan Software" ) )
            {
                using ( var smssAddinKey = laanSoftwareKey.CreateSubKey( "SMSS Addin" ) )
                {
                    object obj = smssAddinKey.GetValue( name );

                    if ( obj != null && smssAddinKey.GetValueKind( name ) == RegistryValueKind.String && typeof(T) == typeof(string) )
                        return (T) obj;

                    return defaultValue;
                }
            }
        }        
        
        protected bool ReadConfigValue( string name, bool defaultValue )
        {
            using ( var laanSoftwareKey = Registry.CurrentUser.CreateSubKey( "Laan Software" ) )
            {
                using ( var smssAddinKey = laanSoftwareKey.CreateSubKey( "SMSS Addin" ) )
                {
                    object obj = smssAddinKey.GetValue( name );

                    if ( obj != null && smssAddinKey.GetValueKind( name ) == RegistryValueKind.DWord )
                        return Convert.ToBoolean( obj );

                    return defaultValue;
                }
            }
        }

        protected void WriteConfigValue<T>( string name, T value )
        {
            using ( var laanSoftwareKey = Registry.CurrentUser.CreateSubKey( "Laan Software" ) )
            {
                using ( var smssAddinKey = laanSoftwareKey.CreateSubKey( "SMSS Addin" ) )
                {
                    if (typeof(T) == typeof(string))
                        smssAddinKey.SetValue(name, value, RegistryValueKind.String);
                    else if (typeof(T) == typeof(bool))
                        smssAddinKey.SetValue( name, value, RegistryValueKind.DWord );
                }
            }
            
        }

        protected T ReadProperty<T>( string category, string page, string property, T defaultValue )
        {
            try
            {
                
                var prop = AddIn.TextDocument.DTE.get_Properties( category, page );
                return ( T )prop.Item( property ).Value;
            }
            catch ( Exception ex )
            {
                AddIn.Error( String.Format("Failed to ReadProperty('{0}', '{1}', '{2}')", category, page, property), ex );
                return defaultValue;
            }
        }

        protected void WriteProperty<T>( string category, string page, string property, T value )
        {
            try
            {
                var prop = AddIn.TextDocument.DTE.get_Properties( category, page );
                prop.Item( property ).Value = value;
            }
            catch ( Exception ex )
            {
                AddIn.Error( String.Format("Failed to WriteProperty('{0}', '{1}', '{2}', '{3})", category, page, property), ex );
            }
        }

    }
}
