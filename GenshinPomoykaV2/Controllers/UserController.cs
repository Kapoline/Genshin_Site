using System;
using System.Linq;
using System.Threading.Tasks;
using GenshinPomoykaV2.Models;
using GenshinPomoykaV2.Data;
using GenshinPomoykaV2.Helpers;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Core.Models;

namespace GenshinPomoykaV2.Controllers
{
    public class UserController: Controller
    {
        private readonly DataRepository _data;
        private readonly  JwtService _service;

        public UserController(DataRepository data,JwtService jwt)
        {
            _data = data;
            _service = jwt;
        }
        
        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Registration([FromForm]RegRequest request)
        {
            if (ModelState.IsValid)
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
                return Redirect("https://localhost:44310/");
            }
            
            return View(request);
        }

        [HttpPost]
        public IActionResult Login([FromForm] AuthRequest request)
        {
            if (ModelState.IsValid)
            {
                var pass = PasswordHashing.Hasher.Encrypt(request.Password);
                var user = Authentication(request.Email, pass);
            
                if (user == null) return View();
            
                var jwt = _service.GenerateToken(user.Id);
                
                Response.Cookies.Append("jwt",jwt,new CookieOptions
                {
                    HttpOnly = true
                });

                return Redirect("https://localhost:44310/");
            }
            return View(request);
        }
        
         public IActionResult LogOut()
         {
             Response.Cookies.Delete("jwt");
             return Redirect("https://localhost:44310/");
         }

        private Account Authentication(string email,string password)
        {
            return _data.Accounts.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
        private Account IsUserExists(string email)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email);
        }
    }
}