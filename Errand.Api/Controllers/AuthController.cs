using Errand.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibraries.Entities;
using SharedLibraries.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Errand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly SqlDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpSignIn model)
        {
            try
            {
                var user = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                user.GeneratePassword(model.Password);
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

            return new OkResult();
        }


        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignUpSignIn model)
        {
            try
            {
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    if (user.ValidatePassword(model.Password))
                    {
                        var th = new JwtSecurityTokenHandler();
                        var expiresDate = DateTime.Now.AddDays(1);

                        var td = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("UserId", user.Id.ToString()),
                                new Claim("Expires", expiresDate.ToString())
                            }),
                            Expires = expiresDate,
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value)),
                                    SecurityAlgorithms.HmacSha512Signature
                                )
                        };

                        var _accessToken = th.WriteToken(th.CreateToken(td));

                        return new OkObjectResult(_accessToken);
                    }
                }
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

            return new BadRequestResult();
        }
    }
}
