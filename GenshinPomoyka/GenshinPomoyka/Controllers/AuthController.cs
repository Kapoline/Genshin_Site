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
        public IActionResult Registration([FromBody] RegRequest request)
        {
            var user = IsUserExists(request.Email);
            if (user != null)
            {
                return Ok("Account already exist");
            }

            user = new Account()
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Password = PasswordHashing.Hasher.Encrypt(request.Password),
                Nickname = request.Nickname,
                Role = "User"
            };
            
            _data.AccountCreate(user);

            return Ok("reg was successful");
        }
        
        /// <summary>
        /// Login Request method
        /// </summary>
        
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var pass = PasswordHashing.Hasher.Encrypt(request.Password);
            var user = AuthenticateUser(request.Email, pass);
            if (user != null)
            {
                var token = JwtGenerate(user);
                return Ok(new
                {
                    access_token=token,
                    request.Email,
                    user.Nickname
                });
            }

            return Unauthorized();
        }

        private Account AuthenticateUser(string email, string password)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email && password == u.Password);
        }

        private Account IsUserExists(string email)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email);
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