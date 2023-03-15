namespace Forum.DTOs
{
    public class PostPreview
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TimeSpan Age { get; set; }
    }

    public class PostThread
    {
        public string Title { get; set; }
        public int ReplyCount { get; set; }
        public int PageCount { get; set; }
        public List<Reply> ReplySet { get; set; }

        public class Reply
        {
            public string User { get; set; }
            public string Content { get; set; }
            public TimeSpan Age { get; set; }
            public int Position { get; set; }
        }
    }

    public class ReplyAdded
    {
        public int Page { get; set; }
        public int Position { get; set; }
    }
}
