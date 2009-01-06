using System;
using System.Collections.Generic;

namespace Laan.SQL.Parser
{
    public interface IProjection
    {
        List<Field> Fields { get; }
    }
}
