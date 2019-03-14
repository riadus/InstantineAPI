namespace InstantineAPI.Data
{
    public class UserChangeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NewPassword { get; set; }
        public bool ChangePassword { get; set; }
        public bool ChangeFirstName { get; set; }
        public bool ChangeLastName { get; set; }
    }
}
