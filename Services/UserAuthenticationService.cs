using Forum.Data;
using Forum.DTOs;
using Forum.Models;
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

        public async Task<ServiceResult<AuthenticatedUser>> AuthenticateUserAsync(string username, string password)
        {
            var result = new ServiceResult<AuthenticatedUser>();
            var user = await context.UserAuthentication.FirstAsync(u => u.UserName == username);
            if (user is null)
            {
                result.Result = Result.Error;
                result.Message = "Invalid authentication.";
                return result;
            }
            var valid = BC.Verify(password, user.PasswordHash);
            if (valid)
            {
                var authenticatedUser = new AuthenticatedUser()
                {
                    UserName = username,
                    UserRoles = new() { "Admin" } // TODO: add roles to the app
                };
                result.Value = authenticatedUser;
                return result;
            }
            result.Result = Result.Error;
            result.Message = "Invalid authentication.";
            return result;
        }

        public async Task<ServiceResult> RegisterNewUserAsync(string username, string password)
        {
            var result = new ServiceResult();
            var userExists = await UserExistsAsync(username);
            if (userExists)
            {
                result.Result = Result.Error;
                result.Message = "Username already exists.";
            }
            else
            {
                var createDate = DateTime.UtcNow;
                var newUser = new User()
                {
                    UserName = username,
                    JoinDate = createDate,
                };
                var hash = BC.HashPassword(password);
                var newUserAuthentication = new UserAuthentication()
                {
                    UserName = username,
                    PasswordHash = hash,
                };
                context.Users.Add(newUser);
                context.UserAuthentication.Add(newUserAuthentication);
                await context.SaveChangesAsync();
            }
            return result;
        }

        private async Task<bool> UserExistsAsync(string username)
        {
            var user = await context.UserAuthentication.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
            {
                return false;
            }
            return true;
        }
    }
}
