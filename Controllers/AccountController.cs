using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(
            SignInManager<IdentityUser> signInManager,  
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Logout() => PartialView();

        [HttpGet]
        public IActionResult RegisterAdmin() => View();

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, bool rememberMe = false, string returnUrl = "/")
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists == null) 
                return View();

            if (await _userManager.CheckPasswordAsync(userExists, model.Password))
            {
                var result = await _signInManager.PasswordSignInAsync(userExists, model.Password, rememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return View();
            }      

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            } 

            if(await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user,UserRoles.User);
            }

            await _signInManager.SignInAsync(user, false);

            return LocalRedirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null) return View();

            IdentityUser admin = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(admin, model.Password);

            if(!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if(!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            
            await _userManager.AddToRoleAsync(admin,UserRoles.User);
            await _userManager.AddToRoleAsync(admin, UserRoles.Admin);

            if (!result.Succeeded)
            {
                return View();
            }

            return LocalRedirect("/Account/Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return LocalRedirect("/");
        }
    }
}

