namespace Forum.Models
{
    public class UserAuthentication
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
