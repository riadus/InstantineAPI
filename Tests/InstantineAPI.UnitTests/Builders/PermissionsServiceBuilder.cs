using InstantineAPI.Core.Database;
using InstantineAPI.Core.Photos;
using InstantineAPI.Photos;

namespace InstantineAPI.UnitTests.Builders
{
    public class PermissionsServiceBuilder
    {
        private IUnitOfWork _unitOfWork;

        public IPermissionsService Build()
        {
            return new PermissionsService(_unitOfWork);
        }

        public PermissionsServiceBuilder WithUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            return this;
        }
    }
}
