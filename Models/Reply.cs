namespace Forum.Models
{
    public class Reply
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
