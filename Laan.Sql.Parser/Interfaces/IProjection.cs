using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser
{
    public interface IProjection
    {
        List<Field> Fields { get; }
    }
}
