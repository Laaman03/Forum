using Forum.Data;
using Forum.DTOs;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Forum.Services
{
    public class UserAuthenticationService
    {
        private ApplicationDbContext context;

        public UserAuthenticationService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<AuthenticatedUser?> AuthenticateUserAsync(string username, string password)
        {
            var user = await context.UserAuthentication.FirstAsync(u => u.UserName == username);
            if (user is null)
            {
                return null;
            }
            var valid = BC.Verify(password, user.PasswordHash);
            if (valid)
            {
                return new AuthenticatedUser()
                {
                    UserName = username,
                    UserRoles = new() { "Admin" } // TODO: add roles to the app
                };
            }
            return null;
        }
    }
}
