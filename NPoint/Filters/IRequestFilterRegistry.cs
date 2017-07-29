using System;
using System.Collections.Generic;

namespace NPoint.Filters
{
    public interface IRequestFilterRegistry
    {
        List<Func<IRequestFilter>> Filters { get; }
    }
}