using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Shopp.Web.Models.DTO;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Shopp.Web.Utility.StaticData;

namespace Shopp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequest = new LoginRequestDTO();
            return View(loginRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            ResponseDTO response = await _authService.LoginAsync(request);
            if (response != null && response.IsSuccessful)
            {
                LoginResponseDTO result = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));
                await SignInUser(result);
                _tokenProvider.SetToken(result.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticData.Roles.Admin.ToString(), Value=StaticData.Roles.Admin.ToString() },
                new SelectListItem{Text=StaticData.Roles.Customer.ToString(), Value=StaticData.Roles.Customer.ToString() }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO request)
        {
            ResponseDTO response = await _authService.RegisterAsync(request);
            ResponseDTO assignRole;
            if (response != null && response.IsSuccessful) 
            {
                if (string.IsNullOrEmpty(request.Role))
                {
                    request.Role = StaticData.Roles.Customer.ToString();
                }
                assignRole = await _authService.AssignRoleAsync(request);
                if (assignRole != null && assignRole.IsSuccessful)
                {
                    TempData["success"] = "Registration succeeded!";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticData.Roles.Admin.ToString(), Value=StaticData.Roles.Admin.ToString() },
                new SelectListItem{Text=StaticData.Roles.Customer.ToString(), Value=StaticData.Roles.Customer.ToString() }
            };
            ViewBag.RoleList = roleList;
            return View(request);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDTO request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(request.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
