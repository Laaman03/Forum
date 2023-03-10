using Forum.Data;
using Forum.DTOs;
using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Services
{
    public class PostService
    {
        private ApplicationDbContext context;

        public PostService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<PostPreview>> GetRecentPostPreviews(int numOfPosts)
        {
            var now = DateTime.UtcNow;
            var contentPreviewLength = 200;
            // we don't have to worry about Substring length here
            // because sql substring just return the whole string
            // if the length argument is greater than the string length
            var posts = await context.Posts
                .OrderByDescending(p => p.ID)
                .Take(numOfPosts)
                .Select(p => new PostPreview()
                {
                    UserName = p.User.UserName,
                    Title = p.Title,
                    Content = p.Content.Substring(0, contentPreviewLength),
                    Age = now - p.CreateDate,
                })
                .ToListAsync();
            foreach (var p in posts.Where(p => p.Content.Length > contentPreviewLength - 3))
            {
                p.Content = p.Content.Substring(0, p.Content.Length - 3) + "...";
            }
            return posts;
        }
    }
}
