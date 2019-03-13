using InstantineAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace InstantineAPI.Middelware.Attributes
{
    public class AuthorizeManagerAttribute : AuthorizeAttribute
    {
        public AuthorizeManagerAttribute()
        {
            Roles = $"{UserRole.Admin},{UserRole.Manager}";
        }
    }
}
