namespace InstantineAPI.UnitTests.Mock
{
    public class AllPermissionsDeniedService : AllSamePermissionsService
    {
        public AllPermissionsDeniedService() : base(false)
        {
        }
    }
}
