using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GenshinPomoykaV2.Models;
using GenshinPomoykaV2.Data;
using GenshinPomoykaV2.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult UserPage()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        
        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword([FromForm] ChangePassword changePassword)
        {
            var user = _data.FindByEmail(User.Identity?.Name);

            if (user.Password == changePassword.Password)
            {
                ModelState.AddModelError("", "Прошлый и нынешний пароли совпадают");
            }

            if (changePassword.Password != changePassword.ConfirmPassword)
            {
                ModelState.AddModelError("", "Пароли различаются");
            }

            var UserAfter = new Account()
            {
                Id = user.Id,
                Email = user.Email,
                Password = PasswordHashing.Hasher.Encrypt(changePassword.Password),
                Nickname = user.Nickname,
                Role = user.Role
            };
            

            _data.ChangePassword(user);
            _data.AccountCreate(UserAfter);
            

            return RedirectToAction("UserPage","User");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([FromForm] RegRequest request)
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

                await Authenticate(request.Email);
                
                return RedirectToAction("Index","Home");
            }
            
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] AuthRequest request)
        {
            if (ModelState.IsValid)
            {
                var pass = PasswordHashing.Hasher.Encrypt(request.Password);
                var user = Authentication(request.Email, pass);
            
                if (user == null) return View();

                await Authenticate(request.Email);

                return RedirectToAction("Index","Home");
            }
            return View(request);
        }
        

        private Account Authentication(string email,string password)
        {
            return _data.Accounts.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
        private Account IsUserExists(string email)
        {
            return _data.Accounts.FirstOrDefault(u => email == u.Email);
        }
        
        private async Task Authenticate(string userName)
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            
            
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}