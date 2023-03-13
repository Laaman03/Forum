using Forum.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Forum.Pages
{
    public class RegisterModel : PageModel
    {
        private UserAuthenticationService authenticationService;

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        public RegisterModel(UserAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        public async Task OnGetAsync(string returnUrl)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = ReturnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var registerUserResult = await authenticationService.RegisterNewUserAsync(Input.UserName, Input.Password);
                if (registerUserResult.Result == Result.Error)
                {
                    ModelState.AddModelError(string.Empty, registerUserResult.Message);
                    return Page();
                }
                // at this point we know the user was created
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, Input.UserName),
                };


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
