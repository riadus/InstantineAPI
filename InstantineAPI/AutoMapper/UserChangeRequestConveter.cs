using AutoMapper;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Data;

namespace InstantineAPI.AutoMapper
{
    internal class UserChangeRequestConveter : ITypeConverter<UserDto, UserChangeRequest>
    {
        public UserChangeRequest Convert(UserDto source, UserChangeRequest destination, ResolutionContext context)
        {
            var userChangeRequest = destination ?? new UserChangeRequest();

            userChangeRequest.FirstName = source.FirstName;
            userChangeRequest.LastName = source.LastName;
            userChangeRequest.NewPassword = source.PasswordDto?.Password;
            userChangeRequest.ChangePassword = source.PasswordDto != null;
            userChangeRequest.ChangeFirstName = source.FirstName != null;
            userChangeRequest.ChangeLastName = source.LastName != null;

            return userChangeRequest;
        }
    }
}