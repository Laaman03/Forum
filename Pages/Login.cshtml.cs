using Forum.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Forum.Pages
{
    public class LoginModel : PageModel
    {
        private UserAuthenticationService authenticationService;

        [BindProperty]
        public InputModel Input { get; set; }

        [FromQuery(Name = "logout")]
        public bool Logout { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        public LoginModel(UserAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            // clear the existing cookie
            await HttpContext.SignOutAsync();

            if (Logout)
            {
                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var userResult = await authenticationService.AuthenticateUserAsync(Input.UserName, Input.Password);
                var user = userResult.Value;
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };
                foreach (var role in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }

            // something failed redisplay the form
            return Page();
        }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Password { get; set; }
        }
    }


}
