// ***********************************************************************************************
//
//  (c) Copyright 2024, Computer Task Group, Inc. (CTG)
//
//  This software is licensed under a commercial license agreement. For the full copyright and
//  license information, please contact CTG for more information.
//
//  Description: CodeNest .
//
// ***********************************************************************************************

using CodeNest.BLL.Service;
using CodeNest.DTO.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeNest.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task GenerateClaimsAsync(string userName)
        {
            ClaimsIdentity identity = new(
                [
                    new (ClaimTypes.Name, userName),
                    new (ClaimTypes.Role, "Users")
                ], CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principal = new(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        public IActionResult ForgotPasswordBasic() => View();
        public IActionResult Login() => View();
        public IActionResult Register() => View();
        /// <summary>
        /// User Login Validation
        /// </summary>
        /// <param name="user"></param>
        /// <returns>the user detail if exist </returns>
        [HttpPost]
        public async Task<IActionResult> Login(UsersDto user)
        {
            UsersDto result = await _userService.Login(user.Name, user.Password);
            if (result != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("userId", result.Id.ToString());
                _httpContextAccessor.HttpContext.Session.SetString("userName", result.Name);
                await GenerateClaimsAsync(result.Name);
                return RedirectToAction("JsonFormatter", "Formatter", new { userId = result.Id });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsersDto user)
        {
            if (ModelState.IsValid)
            {
                UsersDto result = await _userService.Register(user);
                if (result != null)
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("userId", result.Id.ToString());
                    _httpContextAccessor.HttpContext?.Session.SetString("userName", result.Name);
                    return RedirectToAction("Login");
                }
            }

            return View(user);
        }

        /// <summary>
        /// User Logout
        /// </summary>
        /// <returns>Redirects to Login page</returns>
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}
