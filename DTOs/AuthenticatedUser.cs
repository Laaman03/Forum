namespace Forum.DTOs
{
    public class AuthenticatedUser
    {
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; }
    }
}
