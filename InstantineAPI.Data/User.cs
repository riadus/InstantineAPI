using System;
using Newtonsoft.Json;

namespace InstantineAPI.Data
{
    public class User : Entity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool InvitationSent { get; set; }
        public bool InvitationAccepted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime SendingDate { get; set; }
        public DateTime AcceptingDate { get; set; }
        public string UserId { get; set; }
        public UserRole Role { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        [JsonIgnore]
        public string RefreshTokenSalt { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   UserId == user.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId);
        }
    }
}
