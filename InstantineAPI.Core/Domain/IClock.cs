using System;

namespace InstantineAPI.Core.Domain
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
