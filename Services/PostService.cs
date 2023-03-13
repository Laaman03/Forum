using Forum.Data;
using Forum.DTOs;
using Forum.Models;
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

        public async Task<ServiceResult<List<PostPreview>>> GetRecentPostPreviews(int numOfPosts)
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
            var result = new ServiceResult<List<PostPreview>>();
            if (posts is null)
            {
                result.Result = Result.Error;
                result.Message = "Could not retreive posts.";
            }
            result.Value = posts;
            return result;
        }

        public async Task<ServiceResult<PostThread>> GetPostThreadAsync(int postId, int page)
        {
            var result = new ServiceResult<PostThread>();
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
                .FirstOrDefaultAsync();
            if (originalPost is null)
            {
                result.Result = Result.Error;
                result.Message = "Could not find post.";
                return result;
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
                .Skip((page - 1) * pageSize - (includedOriginalPost ^ 1))
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
            var thread = new PostThread()
            {
                Title = originalPost.Title,
                ReplySet = threadReplies,
                ReplyCount = threadReplies.Count,
            };
            result.Value = thread;
            return result;
        }

        public async Task<ServiceResult<int>> SubmitPostAsync(string userName, string title, string content)
        {
            var result = new ServiceResult<int>();
            var userPosts = await context.Users
                .Where(u => u.UserName == userName)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync();
            if (userPosts is null)
            {
                result.Result = Result.Error;
                result.Message = "User does not exist.";
                return result;
            }
            var newPost = new Post()
            {
                Title = title,
                Content = content,
                CreateDate = DateTime.UtcNow,
            };
            userPosts.Posts.Add(newPost);
            await context.SaveChangesAsync();

            var postID = await context.Posts
                .Where(p => p.User.UserName == userName)
                .OrderByDescending(p => p.ID)
                .Select(p => p.ID)
                .FirstOrDefaultAsync();

            result.Value = postID;
            return result;
        }

        public async Task<ServiceResult<ReplyAdded>> AddReplyAsync(int postID, string userName, string replyContent)
        {
            var result = new ServiceResult<ReplyAdded>();
            var post = await context.Posts
                .Where(p => p.ID == postID)
                .Include(p => p.Replies)
                .FirstOrDefaultAsync();
            if (post is null)
            {
                result.Result = Result.Error;
                result.Message = "Invalid post ID.";
                return result;
            }
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName== userName);
            if (user is null)
            {
                result.Result = Result.Error;
                result.Message = "Invalid user.";
                return result;
            }
            var newReply = new Reply()
            {
                User = user,
                Content = replyContent,
                CreateDate = DateTime.UtcNow,
            };
            post.Replies.Add(newReply);
            var replyCount = 2 + await context.Replies
                .Where(r => r.Post.ID == postID)
                .CountAsync();

            var page = (replyCount / (pageSize + 1)) + 1;
            var position = ((replyCount - 1) % pageSize) + 1;

            await context.SaveChangesAsync();

            result.Value = new ReplyAdded()
            {
                Page = page,
                Position = position
            };
            return result;
        }
    }
}
