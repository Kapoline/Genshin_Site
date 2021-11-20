using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GenshinPomoyka.Models;
using GenshinPomoyka.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetTaste;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ess;

namespace GenshinPomoyka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> authOptions;

        public AuthController(IOptions<AuthOptions> authOptions)
        {
            this.authOptions = authOptions;
        }
        
        /// <summary>
        /// This list just a local data example for test
        /// </summary>
        private List<User> Users=>new List<User>
        {
            new User()
            {
                Id = Guid.Parse("8e7eb047-e1e0-4801-ba41-f8360a9e64a7"),
                Email = "shiakyo@yandex.ru",
                Password = "zxc123cxz123",
                Nickname = "shikayo",
                Role =  Roles.User
                
            }
        };
        
        /// <summary>
        /// Register request method
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        [Route("registration")]
        [HttpPost]
        public IActionResult Registration(string email, string password)
        {
            return null; //later  
        }
        
        /// <summary>
        /// Login Request method
        /// </summary>
        
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var user = AuthenticateUser(request.Email, request.Password);
            if (user != null)
            {
                var token = JwtGenerate(user);
                return Ok(new
                {
                    access_token=token
                });
            }

            return Unauthorized();
        }

        private User AuthenticateUser(string email, string password)
        {
            return Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        }

        
        /// <summary>
        /// Method for generate jwt token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string JwtGenerate(User user)
        {
            var authParams = authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("role",user.Role.ToString())
            };
            

            var token = new JwtSecurityToken(
                authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifeTime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        
    }
}