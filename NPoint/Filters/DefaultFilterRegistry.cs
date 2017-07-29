using System;
using System.Collections.Generic;

namespace NPoint.Filters
{
    public class DefaultFilterRegistry : IRequestFilterRegistry
    {
        public List<Func<IRequestFilter>> Filters
        {
            get
            {
                return new List<Func<IRequestFilter>>();
            }
        }
    }
}
