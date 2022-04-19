using System.Threading.Tasks;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails([FromHeader]int userId,[FromHeader]string securityKey)
        {
            var result =  await _userService.GetUserDetails(userId,securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInformations([FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _userService.GetUserInformations(userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


    }
}