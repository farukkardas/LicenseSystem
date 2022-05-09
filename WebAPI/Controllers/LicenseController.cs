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

        public LicenseController(IKeyLicenseService keyLicenseService)
        {
            _keyLicenseService = keyLicenseService;
        }

        [HttpPost("NewLicense")]
        public async Task<IActionResult> AddNewLicense(int keyEnd, int applicationId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.CreateLicenseKey(keyEnd, applicationId, userId, securityKey);
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


        [HttpGet("GetLicenseByAppId")]
        public async Task<IActionResult> GetLicenseByAppId(int applicationId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.GetLicensesByAppId(applicationId, userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
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

  [HttpPost("DeleteAllLicensesByAppId")]
          public async Task<IActionResult> DeleteAllLicenses(int applicationId,[FromHeader] int userId, [FromHeader] string securityKey)
        {
            var deleteAllKeys = await _keyLicenseService.DeleteAllKeysByAppId(applicationId,userId, securityKey);

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

             [HttpPost("ResetAllLicensesByAppId")]
        public async Task<IActionResult> ResetAllHwids(int applicationId,[FromHeader] int userId, [FromHeader] string securityKey)
        {
            var deleteAllKeys = await _keyLicenseService.ResetAllHwidByAppId(applicationId,userId, securityKey);

            if (deleteAllKeys.Success)
            {
                return Ok(deleteAllKeys);
            }

            return BadRequest(deleteAllKeys);
        }

        [HttpPost("CheckLicense")]
        public async Task<IActionResult> CheckLicense(string keyLicense,string hwid)
        {
            var requestIp = OnGetAsync();

            var checkLicense = await _keyLicenseService.CheckLicense(keyLicense, hwid, requestIp);

            if (checkLicense.Success)
            {
                return Ok(checkLicense);
            }

            return BadRequest(checkLicense);
        }

        [HttpPost("ExtendAllLicenses")]
        public async Task<IActionResult> ExtendAllKeys(int timeSelection,int dateOption,int applicationId,[FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.ExtendAllKeys(timeSelection, dateOption, applicationId, userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("ExtendLicense")]
        public async Task<IActionResult> ExtendLicense(int timeSelection, int dateOption, int keyId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.ExtendSingleKey(timeSelection, dateOption, keyId, userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("DeleteUnusedKeys")]
        public async Task<IActionResult> DeleteUnusedKeys(int applicationId,[FromHeader] int userId, [FromHeader] string securityKey)
        {
            var result = await _keyLicenseService.DeleteUnusedKeys(applicationId,userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // delete expired keys
        [HttpPost("DeleteExpiredKeys")]
        public async Task<IActionResult> DeleteExpiredKeys(int? applicationId, [FromHeader] int userId, [FromHeader] string securityKey)
        {
   
         var result = await _keyLicenseService.DeleteExpiredKeys(applicationId, userId, securityKey);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public string OnGetAsync()
        {
            string clientAdress = "";
            // Retrieve client IP address through HttpContext.Connection
            clientAdress = HttpContext.Connection.RemoteIpAddress?.ToString();
            return clientAdress;
        }
    }
}