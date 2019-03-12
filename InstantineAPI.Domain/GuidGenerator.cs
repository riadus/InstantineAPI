using System;
using InstantineAPI.Core.Domain;

namespace InstantineAPI.Domain
{
    public class GuidGenerator : IGuid
    {
        private readonly Func<Guid> _guidFunc;

        public GuidGenerator(Func<Guid> guidFunc)
        {
            _guidFunc = guidFunc;
        }

        public Guid NewGuid()
        {
            return _guidFunc();
        }
    }
}