using System;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public interface IFormattingEngine
    {
        string Execute( string sql );
        int TabSize { get; set; }
        bool UseTabChar { get; set; }
    }
}
