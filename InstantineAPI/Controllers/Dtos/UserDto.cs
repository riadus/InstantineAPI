namespace InstantineAPI.Controllers.Dtos
{
    public class UserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PhotoDto
    {
        public byte[] Image { get; set; }
        public string UserId { get; set; }
    }

    public class CommentDto
    {
        public string CommentorId { get; set; }
        public string Message { get; set; }
    }

    public class AlbumDto
    {
        public string Name { get; set; }
    }

    public class AuthenticationDto
    {
        public string Code { get; set; }
    }
}
