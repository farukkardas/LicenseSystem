using System.Threading.Tasks;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : Controller
    {
        private ILogService _logService;

        // GET
        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetLogs([FromHeader] int userId)
        {
            var result = await _logService.GetAll(userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}