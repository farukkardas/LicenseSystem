using System.Threading.Tasks;
using Business.Abstract;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async  Task<IActionResult>  Login(UserLoginDto userLoginDto)
        {
            var userToLogin =  await _authService.Login(userLoginDto);

            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin);
            }

            var result =  await _authService.CreateAccessToken(userToLogin.Data);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var registerResult = await _authService.Register(userRegisterDto, userRegisterDto.Password);
            var result = await _authService.CreateAccessToken(registerResult.Data);
            result.Message = "Successfully registered!";

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        [HttpPost("CheckSkOutdated")]
        public async Task<ActionResult> CheckSkOutdated([FromForm]int userId)
        {
            var result = await _authService.CheckSecurityKeyOutdated(userId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    
    }
}