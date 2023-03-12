using Forum.Data;
using Forum.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Forum.Services
{
    public class PostService
    {
        private static int pageSize = 10;
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
            // because sql substring will return the whole string
            // if the length argument is greater than the string length
            var posts = await context.Posts
                .OrderByDescending(p => p.ID)
                .Take(numOfPosts)
                .Select(p => new PostPreview()
                {
                    ID = p.ID,
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

        public async Task<PostThread?> GetPostThreadAsync(int postId, int page)
        {
            page = Math.Max(page, 1);
            var now = DateTime.UtcNow;
            var originalPost = await context.Posts
                .Where(p => p.ID == postId)
                .Select(p => new
                {
                    User = p.User.UserName,
                    p.Title,
                    p.Content,
                    Age = now - p.CreateDate,
                    ReplyCount = p.Replies.Count,
                })
                .FirstAsync();
            if (originalPost is null)
            {
                return null;
            }
            int includedOriginalPost = 0;
            var threadReplies = new List<PostThread.Reply>();
            if (page == 1)
            {
                includedOriginalPost = 1;
                threadReplies.Add(new PostThread.Reply()
                {
                    User = originalPost.User,
                    Content = originalPost.Content,
                    Age = originalPost.Age,
                });
            }

            var replies = await context.Replies
                .Where(r => r.Post.ID == postId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize - includedOriginalPost)
                .OrderBy(r => r.ID)
                .Select(r => new PostThread.Reply()
                {
                    User = r.User.UserName,
                    Content = r.Content,
                    Age = now - r.CreateDate,
                })
                .ToListAsync();
            threadReplies.AddRange(replies);
            for (var i = 0; i < threadReplies.Count; i++)
            {
                threadReplies[i].Position = ((page-1) * pageSize) + i + 1;
            }
            return new PostThread()
            {
                Title = originalPost.Title,
                ReplySet = threadReplies,
                ReplyCount = threadReplies.Count,
            };
        }
    }
}
