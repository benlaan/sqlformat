using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Laan.SQL.Parser
{
    public interface ITokenizer
    {
        bool HasMoreTokens { get; }
        bool IsNextToken( params string[] tokenSet );

        bool TokenEquals( string value );
        void ReadNextToken();
        Token Current { get; }
        void ExpectToken( string token );
        void ExpectTokens( string[] tokens );
        Position Position { get; }
        BracketStructure ExpectBrackets();
    }
}
