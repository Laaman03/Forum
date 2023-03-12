using Forum.DTOs;
using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Forum.Pages.Post
{
    public class IndexModel : PageModel
    {
        private PostService postService;

        [FromQuery(Name = "id")]
        public int PostID { get; set; }

        [FromQuery(Name = "page")]
        public int PageNum { get; set; } = 1;

        public PostThread? PostThread { get; set; }

        public IndexModel(PostService postService)
        {
            this.postService = postService;
        }
        public async Task<IActionResult> OnGet()
        {
            if (PostID > 0)
            {
                PostThread = await postService.GetPostThreadAsync(PostID, PageNum);
            }
            if (PostThread is null)
            {
                return Content("No such item.");
            }
            return Page();
        }
    }
}
