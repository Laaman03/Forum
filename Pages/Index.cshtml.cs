using Forum.DTOs;
using Forum.Models;
using Forum.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Forum.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PostService postService;

    public List<PostPreview> Posts { get; set; }

    public IndexModel(ILogger<IndexModel> logger, PostService postService)
    {
        _logger = logger;
        this.postService = postService;
    }

    public async Task OnGet()
    {
        var posts = await postService.GetRecentPostPreviews(10);
        Posts = posts.Value;
    }
}
