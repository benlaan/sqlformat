using System;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public interface IFormattingEngine
    {
        string Execute( string sql );
        int TabSize { get; set; }
        bool UseTabChar { get; set; }
    }
}
