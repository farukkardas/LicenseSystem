using System.Threading.Tasks;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;
using Entities.Concrete;

namespace WebAPI.Controllers
{
    //create class ApplicationController
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _applicationService.GetAll();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromHeader]int userId, [FromHeader]string securityKey)
        {
          
            var result = await _applicationService.GetById(userId,securityKey);

            if(!result.Success)
            {
                return BadRequest(result);
            }
         
            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromHeader]int userId,[FromHeader]string securityKey,Application application)
        {
            var result = await _applicationService.Add(userId,securityKey,application);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}