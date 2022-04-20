using System;
using System.Threading.Tasks;
using Business.Abstract;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("ApiCorsPolicy")]
    public class LicenseController : Controller
    {
        private readonly IKeyLicenseService _keyLicenseService;
        public string ClientIPAddr { get; private set; }

        public LicenseController(IKeyLicenseService keyLicenseService)
        {
            _keyLicenseService = keyLicenseService;
        }

        [HttpPost("NewLicense")]
        public async Task<IActionResult> AddNewLicense(int keyEnd, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.CreateLicenseKey(keyEnd, userId, securityKey);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpPost("DeleteLicense")]
        public async Task<IActionResult> DeleteLicense(int keyId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var registerResult = await _keyLicenseService.Delete(keyId, userId, securityKey);

            if (registerResult.Success)
            {
                return Ok(registerResult);
            }

            return BadRequest(registerResult);
        }


        [HttpGet("GetLicenses")]
        public async Task<IActionResult> GetLicenses([FromHeader] int userId, [FromHeader] string securityKey)
        {
            var getLicenses = await _keyLicenseService.GetLicenses(userId, securityKey);

            if (getLicenses.Success)
            {
                return Ok(getLicenses);
            }

            return BadRequest(getLicenses);
        }

        [HttpPost("ResetHwid")]
        public async Task<IActionResult> ResetHwid(int keyId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var getLicenses = await _keyLicenseService.ResetHwid(keyId, userId, securityKey);

            if (getLicenses.Success)
            {
                return Ok(getLicenses);
            }

            return BadRequest(getLicenses);
        }

        [HttpPost("DeleteAllLicenses")]
        public async Task<IActionResult> DeleteAllLicenses([FromHeader] int userId, [FromHeader] string securityKey)
        {
            var deleteAllKeys = await _keyLicenseService.DeleteAllKeys(userId, securityKey);

            if (deleteAllKeys.Success)
            {
                return Ok(deleteAllKeys);
            }

            return BadRequest(deleteAllKeys);
        }

        [HttpPost("ResetAllLicenses")]
        public async Task<IActionResult> ResetAllHwids([FromHeader] int userId, [FromHeader] string securityKey)
        {
            var deleteAllKeys = await _keyLicenseService.ResetAllHwid(userId, securityKey);

            if (deleteAllKeys.Success)
            {
                return Ok(deleteAllKeys);
            }

            return BadRequest(deleteAllKeys);
        }

        [HttpPost("CheckLicense")]
        public async Task<IActionResult> CheckLicense([FromForm] string keyLicense, [FromForm] string hwid)
        {
            var requestIp = OnGetAsync();

            var checkLicense = await _keyLicenseService.CheckLicense(keyLicense, hwid,ClientIPAddr);

            if (checkLicense.Success)
            {
                return Ok(checkLicense);
            }

            return BadRequest(checkLicense);
        }


        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve client IP address through HttpContext.Connection
            ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok();
        }
    }
}