using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopp.MessageBus;
using Shopp.Services.AuthAPI.Models.DTO;
using Shopp.Services.AuthAPI.Service.IService;

namespace Shopp.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _response;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _response = new ResponseDTO();
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO request)
        {
            var errorMessage = await _authService.Register(request);
            if (!string.IsNullOrEmpty(errorMessage)) 
            { 
                _response.IsSuccessful = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            //await _messageBus.PublishMessage(request.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _authService.Login(request);
            if (response.User == null)
            {
                _response.IsSuccessful = false;
                _response.Message = "Username or Password is incorrect!";
                return BadRequest(_response);
            }
            _response.Result = response;
            return Ok(_response);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO request)
        {
            var isAssignRoleSuccessful = await _authService.AssignRole(request.Email,request.Role.ToUpper());
            if (!isAssignRoleSuccessful)
            {
                _response.IsSuccessful = false;
                _response.Message = "Error Encountered while assigning role!";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
