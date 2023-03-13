using Azure.Identity;
using Forum.DTOs;
using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Forum.Pages.Post
{
    public class IndexModel : PageModel
    {
        private PostService postService;

        [FromQuery(Name = "id")]
        public int PostID { get; set; }

        [FromQuery(Name = "pagenum")]
        public int PageNum { get; set; } = 1;

        [BindProperty]
        public string ReplyContent { get; set; }

        public PostThread? PostThread { get; set; }

        public IndexModel(PostService postService)
        {
            this.postService = postService;
        }
        public async Task<IActionResult> OnGet()
        {
            if (PostID > 0)
            {
                var thread = await postService.GetPostThreadAsync(PostID, PageNum);
                PostThread = thread.Value;
            }
            if (PostThread is null)
            {
                return Content("No such item.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            if (PostID <= 0 || ReplyContent == string.Empty) 
            {
                return BadRequest();
            }
            var userName = HttpContext.User.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name).Value!;
            var res = await postService.AddReplyAsync(PostID, userName, ReplyContent);
            if (res.Value is null)
            {
                ModelState.AddModelError(string.Empty, res.Message);
                return Page();
            }
            var addedReply = res.Value;
            return RedirectToPage(
                "/Post/Index", 
                null, 
                new { id = PostID, pagenum = addedReply.Page }, 
                addedReply.Position.ToString()
            );
        }
    }
}
