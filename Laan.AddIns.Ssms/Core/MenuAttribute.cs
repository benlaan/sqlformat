using System;

namespace Laan.AddIns.Actions
{
    public class MenuAttribute : Attribute
    {
        public string CommandBar { get; set; }
        public string Menu { get; set; }

        public MenuAttribute(string menu)
        {
            CommandBar = "MenuBar";
            Menu = menu.Contains("&") ? menu : "&" + menu;
        }
    }
}