using Cko.Common.Infrastructure.Interfaces;
using System;

namespace Cko.Common.Core.Providers
{
    public class StaticValuesProvider : IStaticValuesProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
