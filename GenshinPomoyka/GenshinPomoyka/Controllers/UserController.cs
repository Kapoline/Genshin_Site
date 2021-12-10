using System;
using System.Linq;
using GenshinPomoyka.Data;
using GenshinPomoyka.Helpers;
using GenshinPomoyka.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenshinPomoyka.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly DataRepository _data;

        /// <summary>
        /// Auth controller constructor
        /// </summary>
        /// <param name="jwtService"></param>
        /// <param name="dataRepository">instance of data connection</param>
        public UserController(JwtService jwtService,DataRepository dataRepository)
        {
            _jwtService = jwtService;
            _data = dataRepository;
        }
        
        /// <summary>
        /// Register request method
        /// </summary>
        /// <returns></returns>
        [Route("registration")]
        [HttpPost]
        public IActionResult Registration([FromBody] RegRequest request)
        {
            var user = IsUserExists(request.Email);
            if (user != null)
            {
                return Conflict("Account already exist");
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
            
            if (user == null) return Unauthorized();
            
            var jwt = _jwtService.GenerateToken(user.Id);
                
            Response.Cookies.Append("jwt",jwt,new CookieOptions
            {
                HttpOnly = true
            });
                
            return Ok(new
            {
                message="success"
            });
        }

        [Route("getuser")]
        [HttpGet]
        public new IActionResult User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);
                
                Guid userId = Guid.Parse(token.Issuer);

                Account user = _data.Accounts.FirstOrDefault(u => u.Id == userId);

                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new
            {
                message = "success"
            });
        }
        
        private Account AuthenticateUser(string email, string password)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email && password == u.Password);
        }

        private Account IsUserExists(string email)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email);
        }
    }
}