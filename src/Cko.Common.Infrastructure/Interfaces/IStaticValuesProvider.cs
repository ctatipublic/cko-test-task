using System;

namespace Cko.Common.Infrastructure.Interfaces
{
    public interface IStaticValuesProvider
    {
        DateTime GetUtcNow();
        Guid GetGuid();
    }
}
