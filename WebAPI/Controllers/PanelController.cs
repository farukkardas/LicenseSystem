using System.Threading.Tasks;
using Business.Abstract;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanelController : Controller
    {
        private readonly IPanelService _panelService;
        public PanelController(IPanelService panelService)
        {
            _panelService = panelService;
        }

        [HttpPost("CreateNewPanel")]
        public async Task<IActionResult> CreateNewPanel(PanelRegisterDto panelRegisterDto,[FromHeader]int userId,[FromHeader] string securityKey)
        {
            var result = await _panelService.CreateNewPanel(panelRegisterDto, userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        [HttpGet("GetUserPanels")]
        public async Task<IActionResult> GetUserPanels([FromHeader]int userId,[FromHeader] string securityKey)
        {
            var result = await _panelService.GetUserPanels(userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        [HttpPost("DisablePanel")]
        public async Task<IActionResult> DisablePanel(int panelId,[FromHeader]int userId,[FromHeader] string securityKey)
        {
            var result = await _panelService.DisablePanel(panelId,userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("GetPanelsByUserId")]
        public async Task<IActionResult> GetPanelsByUserId([FromHeader]int userId,[FromHeader] string securityKey,int applicationId)
        {
            var result = await _panelService.GetPanelsByUserId(userId, securityKey,applicationId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}