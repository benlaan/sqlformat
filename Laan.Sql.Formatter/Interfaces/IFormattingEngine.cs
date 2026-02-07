using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public interface IFormattingEngine
    {
        string Execute( string sql );
        FormattingOptions Options { get; }
        
        // Legacy properties for backward compatibility
        int TabSize { get; set; }
        bool UseTabChar { get; set; }
    }
}
