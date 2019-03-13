using InstantineAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace InstantineAPI.Middelware.Attributes
{
    public class AuthorizeAdminOnlyAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminOnlyAttribute()
        {
            Roles = UserRole.Admin.ToString();
        }
    }
}
