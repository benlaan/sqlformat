using System;

namespace Laan.AddIns.Actions
{
    public class MenuAttribute : Attribute
    {
        public string CommandBar { get; set; }
        public string Menu { get; set; }

        public MenuAttribute( string commandBar, string menu )
        {
            CommandBar = commandBar;
            Menu = menu;
        }
    }

    public class MenuBarToolsMenuAttribute : MenuAttribute
    {
        public MenuBarToolsMenuAttribute( ) : base( "MenuBar", "Tools" )
        {
        }
    }

    public class ResultsMenuAttribute : MenuAttribute
    {
        public ResultsMenuAttribute()
            : base( "SQL Results Messages Tab Context", null )
        {
        }
    }
}