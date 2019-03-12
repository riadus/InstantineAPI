using System;
using InstantineAPI.Core.Domain;

namespace InstantineAPI.Domain
{
    public class Clock : IClock
    {
        private readonly Func<DateTime> _funcDateTime;

        public Clock(Func<DateTime> funcDateTime)
        {
            _funcDateTime = funcDateTime;
        }
        public DateTime UtcNow => _funcDateTime();
    }
}