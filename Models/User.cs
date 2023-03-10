namespace Forum.Models
{
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public DateTime JoinDate { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Reply> Replies { get; set; }

    }
}
