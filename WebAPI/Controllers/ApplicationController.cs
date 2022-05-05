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
        public async Task<IActionResult> GetById([FromHeader] int userId, [FromHeader] string securityKey)
        {

            var result = await _applicationService.GetById(userId, securityKey);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromHeader] int userId, [FromHeader] string securityKey, Application application)
        {
            var result = await _applicationService.Add(userId, securityKey, application);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromHeader] int userId, [FromHeader] string securityKey, int applicationId)
        {
            var result = await _applicationService.Delete(userId, securityKey, applicationId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("DisableApplication")]
        public async Task<IActionResult> DisableApplication([FromHeader] int userId, [FromHeader] string securityKey, int applicationId)
        {
            var result = await _applicationService.DisableApplication(userId, securityKey, applicationId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("SetApplicationPrices")]
        public async Task<IActionResult> SetApplicationPrices(SetApplicationPrices applicationPrice, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _applicationService.SetApplicationPrices(applicationPrice, userId, securityKey);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}