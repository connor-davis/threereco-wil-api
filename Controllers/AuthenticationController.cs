using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;
using System.Security.Claims;
using three_api.Lib.Database;
using three_api.Lib.Models;
using three_api.Lib.Services;
using three_api.Lib.Utilities;

namespace three_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController(DatabaseContext context, AuthenticationService authenticationService) : Controller
    {
        // POST: Authentication/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([Bind("Email,Password")] UserAuthentication userAuthentication)
        {
            if (ModelState.IsValid)
            {
                if (await UserExistsWithEmail(userAuthentication.Email))
                    return Json(new
                    {
                        Message = "The user already exists."
                    });

                var user = new User(Guid.NewGuid(), userAuthentication.Email)
                {
                    Password = Password.HashPassword(userAuthentication.Password),
                    Roles = [Roles.Guest]
                };

                context.Add(user);
                
                await context.SaveChangesAsync();

                var AccessToken = authenticationService.GenerateToken(user);

                return Json(new
                {
                    User = user,
                    AccessToken
                });
            }

            return Json(new
            {
                Message = "Please ensure that you have provided valid authentication details."
            });
        }

        // POST: Authentication/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([Bind("Email,Password")] UserAuthentication userAuthentication)
        {
            if (ModelState.IsValid)
            {
                var User = await context.Users.Where(User => User.Email == userAuthentication.Email).FirstOrDefaultAsync();
                
                if (User == null) 
                    return Json(new
                    {
                        Message = "The user does not exist."
                    });

                if (!Password.VerifyPassword(userAuthentication.Password, User.Password))
                    return Json(new
                    {
                        Message = "Invalid authentication details."
                    });

                var AccessToken = authenticationService.GenerateToken(User);

                return Json(new
                {
                    User,
                    AccessToken
                });
            }

            return Json(new
            {
                Message = "Please ensure that you have provided valid authentication details."
            });
        }

        // GET: Authentication/Me
        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (Email == null) return Unauthorized();

            var UserFound = await context.Users.Where(User => User.Email == Email).FirstOrDefaultAsync();

            if (UserFound == null) return Unauthorized();

            return Json(UserFound);
        }

        private async Task<bool> UserExistsWithEmail(string userEmail)
        {
            var User = await context.Users.Where(User => User.Email == userEmail).FirstOrDefaultAsync();

            return User != null;
        }

        private async Task<bool> UserExistsWithId(Guid userId)
        {
            var User = await context.Users.Where(User => User.Id == userId).FirstOrDefaultAsync();

            return User != null;
        }
    }
}
