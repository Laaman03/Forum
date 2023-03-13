using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Forum.Pages.Post
{
    public class SubmitModel : PageModel
    {
        private PostService postService;

        [BindProperty]
        public InputModel Input { get; set; }

        public SubmitModel(PostService postService)
        {
            this.postService = postService;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value!;
            var newPostIDResult = await postService.SubmitPostAsync(user, Input.Title, Input.Content);

            if (newPostIDResult.Result == Result.Error)
            {
                ModelState.AddModelError(string.Empty, newPostIDResult.Message);
                return Page();
            }

            return RedirectToPage("/Post/Index", new { id = newPostIDResult.Value, pagenum = 1 });
        }
    }

    public class InputModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
