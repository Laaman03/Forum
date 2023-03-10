using Microsoft.Data.SqlClient.DataClassification;

namespace Forum.DTOs
{
    public class PostPreview
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TimeSpan Age { get; set; }
    }
}
