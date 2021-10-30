using Cko.Common.Infrastructure.Interfaces;
using System;

namespace Cko.Common.Core.Providers
{
    public class StaticValuesProvider : IStaticValuesProvider
    {
        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
