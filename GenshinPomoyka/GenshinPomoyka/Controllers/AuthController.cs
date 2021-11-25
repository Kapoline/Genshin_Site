using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GenshinPomoyka.Data;
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
        private readonly DataRepository _data;
        
        /// <summary>
        /// Auth controller constructor
        /// </summary>
        /// <param name="authOptions">jwt token services</param>
        /// <param name="dataRepository">instance of data connection</param>
        public AuthController(IOptions<AuthOptions> authOptions,DataRepository dataRepository)
        {
            this.authOptions = authOptions;
            _data = dataRepository;
        }
        
        
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

        private Account AuthenticateUser(string email, string password)
        {
            return _data.Users.FirstOrDefault(u => email == u.Email && password == u.Password);
        }

        
        /// <summary>
        /// Method for generate jwt token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string JwtGenerate(Account user)
        {
            var authParams = authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.Password),
                
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