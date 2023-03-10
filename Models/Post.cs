namespace Forum.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
