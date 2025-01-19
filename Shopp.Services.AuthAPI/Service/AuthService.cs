using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopp.Services.AuthAPI.Data;
using Shopp.Services.AuthAPI.Models;
using Shopp.Services.AuthAPI.Models.DTO;
using Shopp.Services.AuthAPI.Service.IService;

namespace Shopp.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJWTTokenGenerator _jwtTokenGenerator;

        public AuthService(AuthDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJWTTokenGenerator tokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = tokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user,roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
        {
            string error = "";
            var user = _db.Users.FirstOrDefault(u => u.UserName.ToLower() == requestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, requestDTO.Password);
            if (user==null || !isValid) 
            {
                return new LoginResponseDTO() { User = null, Token = String.Empty };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user,roles);

            UserDTO currentUser = new UserDTO()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDTO response = new LoginResponseDTO()
            {
                User = currentUser,
                Token = token,
            };
            return response;
        }

        public async Task<string> Register(RegistrationRequestDTO requestDTO)
        {
            string error = "";
            ApplicationUser user = new ApplicationUser()
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,
                NormalizedEmail = requestDTO.Email.ToUpper(),
                Name = requestDTO.Name,
                PhoneNumber = requestDTO.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, requestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.Users.First(u => u.UserName == requestDTO.Email);

                    UserDTO returnedUser = new UserDTO()
                    {
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        ID = userToReturn.Id,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                }
                else
                {
                    error = result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }
    }
}
