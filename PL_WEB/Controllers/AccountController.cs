using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BLL.Interfaces;
using BLL.Infrastructure;
using BLL.DTO;
using PL_WEB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace PL_WEB.Controllers
{
    public class AccountController : Controller
    {
        IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Login(string ReturnUrl = "/")
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel objLoginModel)
        {
            if (ModelState.IsValid)
            {
                UserDTO user;
                try
                {
                    user = _userService.GetUser(objLoginModel.UserName, objLoginModel.Password);
                }
                catch(ValidationException ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objLoginModel);
                }
                if(user != null)
                {
                    
                    var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier,Convert.ToString(user.Id)),
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, user.Role),
                    };

         
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                    var principal = new ClaimsPrincipal(identity);
                    
                    await HttpContext.SignInAsync(
                       principal, new AuthenticationProperties() { IsPersistent = objLoginModel.RememberLogin });

                    return LocalRedirect(objLoginModel.ReturnUrl);
                }
            }
            return View(objLoginModel);
        }


        public IActionResult Register(string ReturnUrl = "/")
        {
            RegistrationModel objRegModel = new RegistrationModel();
            objRegModel.ReturnUrl = ReturnUrl;
            return View(objRegModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegistrationModel objRegModel)
        {
            if (ModelState.IsValid)
            {
                if (objRegModel.Password != objRegModel.PasswordConfirm)
                {
                    ViewBag.Message = "Password and PasswordConfirm are different!";
                    return View(objRegModel);
                }
                var userDTO = new UserDTO { Login = objRegModel.UserName, Password = objRegModel.Password };
                try
                {
                    _userService.RegUser(userDTO);
                }
                catch(ValidationException ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objRegModel);
                }

                ViewBag.Message = "Registration is successful";
                return LocalRedirect(objRegModel.ReturnUrl);
            }
            return View(objRegModel);
        }

        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page
            return LocalRedirect("/");
        }

        protected override void Dispose(bool disposing)
        {
            _userService.Dispose();
            base.Dispose(disposing);
        }
    }
}
