using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Core.Aspects.Autofac.Caching;

namespace Business.Concrete
{
    public class KeyLicenseService : IKeyLicenseService
    {
        private readonly IKeyLicenseDal _keyLicenseDal;
        private readonly IApplicationDal _applicationDal;
        private readonly IUserDal _userDal;
        private readonly IAuthService _authService;
        private readonly IPanelDal _panelDal;
        private ILogService _logService;

        public KeyLicenseService(IKeyLicenseDal keyLicenseDal, IUserDal userDal, IAuthService authService,
            IPanelDal panelDal, ILogService logService, IApplicationDal applicationDal)
        {
            _keyLicenseDal = keyLicenseDal;
            _userDal = userDal;
            _applicationDal = applicationDal;
            _authService = authService;
            _panelDal = panelDal;
            _logService = logService;
        }

        [CacheAspect(60)]
        public async Task<IDataResult<List<KeyLicense>>> GetAll()
        {
            var result = await _keyLicenseDal.GetAll();

            return new SuccessDataResult<List<KeyLicense>>(result);
        }

        [CacheAspect(60)]
        public async Task<IDataResult<KeyLicense>> GetById(string authKey)
        {
            var result = await _keyLicenseDal.Get(k => k.AuthKey == authKey);

            return new SuccessDataResult<KeyLicense>(result);
        }

        [CacheRemoveAspect("IKeyLicenseService.Get")]
        [SecuredOperations("admin,reseller,localseller")]
        public async Task<IResult> Add(KeyLicense key, int userId, string securityKey)
        {
            await _keyLicenseDal.Add(key);
            return new SuccessResult("Key successfully created.");
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheAspect]
        public async Task<IDataResult<List<KeyLicense>>> GetLicensesByAppId(int appId, int userId, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey), await CheckIfApplicationOwnerTrue(appId, userId));

            if (checkConditions != null)
            {
                return new ErrorDataResult<List<KeyLicense>>(checkConditions.Message);
            }

            var result = await _keyLicenseDal.GetAll(k => k.ApplicationId == appId);

            return new SuccessDataResult<List<KeyLicense>>(result);
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> Delete(int keyId, int id, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(id, securityKey));

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }

            var getKey = await _keyLicenseDal.Get(k => k.Id == keyId);
            if (getKey == null) return new ErrorResult("Key not found!");
            await _keyLicenseDal.Delete(getKey);
            var log = new Log { Success = true, Message = $"{getKey.AuthKey} deleted successfully.", Date = DateTime.Now, OwnerId = getKey.OwnerId };
            await _logService.Add(log);
            return new SuccessResult("Key successfully deleted!");
        }


        [SecuredOperations("admin,reseller,localseller")]
        [CacheAspect]
        public async Task<IDataResult<List<KeyLicense>>> GetLicenses(int id, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(id, securityKey));

            if (checkConditions != null)
            {
                return new ErrorDataResult<List<KeyLicense>>(checkConditions.Message);
            }

            List<KeyLicense?> result = new List<KeyLicense?>();

            var getUserApplications = await _applicationDal.GetAll(a => a.OwnerId == id);
            var getLicenses = await _keyLicenseDal.GetAll(u => u.OwnerId == id);

            foreach (var license in getLicenses)
            {
                result.Add(license);
            }

            foreach (var application in getUserApplications)
            {
                var getLicensesByAppId = await _keyLicenseDal.GetAll(u => u.ApplicationId == application.Id);
                foreach (var license in getLicensesByAppId)
                {
                    result.Add(license);
                }
            }

            List<KeyLicense?> lastResult = result.ToArray().GroupBy(x => x.Id).Select(x => x.First()).ToList();


            return new SuccessDataResult<List<KeyLicense>>(lastResult);
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> ResetHwid(int keyId, int userId, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey));

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }

            var getKey = await _keyLicenseDal.Get(k => k.Id == keyId);
            if (getKey == null) return new ErrorResult("Key not found for hwid reset!");
            getKey.Hwid = null;
            getKey.IsOwned = false;
            await _keyLicenseDal.Update(getKey);

            var log = new Log { Success = true, Message = $"{getKey.AuthKey} hwid reset successfully.", Date = DateTime.Now, OwnerId = getKey.OwnerId };
            await _logService.Add(log);
            return new SuccessResult("Hwid reset successfully");
        }

        public async Task<IResult> CheckLicense(string keyLicense, string hwid, string requestIp)
        {
            var getKey = await _keyLicenseDal.Get(k => k.AuthKey == keyLicense);
            var checkConditions = BusinessRules.Run(await CheckKeyAndHwidIsValid(keyLicense, hwid, getKey?.ExpirationDate, requestIp));

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }


            getKey.IsOwned = true;
            getKey.Hwid = hwid;
            await _keyLicenseDal.Update(getKey);
            var log = new Log { Success = true, OwnerId = getKey.OwnerId, Date = DateTime.Now, Message = $"Key logged successfully {keyLicense} and IP {requestIp}" };
            await _logService.Add(log);
            return new SuccessResult($"Successfully authorized. Expiry: {getKey.ExpirationDate}");
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> DeleteAllKeys(int userId, string securityKey)
        {
            var businessConditions = BusinessRules.Run(
                await _authService.CheckUserSecurityKeyValid(userId, securityKey)
                );

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }


            var allKeys = await _keyLicenseDal.GetAll(k => k.OwnerId == userId);

            foreach (var t in allKeys)
            {
                await _keyLicenseDal.Delete(t);
            }

            return new SuccessResult("All keys deleted successfully");
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> DeleteAllKeysByAppId(int appId, int userId, string securityKey)
        {
            var businessConditions = BusinessRules.Run(
                await _authService.CheckUserSecurityKeyValid(userId, securityKey), await CheckIfApplicationOwnerTrue(appId, userId));

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }

            var allKeys = await _keyLicenseDal.GetAll(k => k.ApplicationId == appId);

            foreach (var t in allKeys)
            {
                await _keyLicenseDal.Delete(t);
            }

            return new SuccessResult("All keys deleted successfully");
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> ResetAllHwid(int userId, string securityKey)
        {

            var businessConditions = BusinessRules.Run(
                await _authService.CheckUserSecurityKeyValid(userId, securityKey));

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }

            var allKeys = await _keyLicenseDal.GetAll(k => k.OwnerId == userId);
            foreach (var key in allKeys)
            {
                key.Hwid = null;
                await _keyLicenseDal.Update(key);
            }

            var log = new Log { Success = true, Message = $"All hwid reset successfully.", Date = DateTime.Now, OwnerId = userId };
            await _logService.Add(log);
            return new SuccessResult("All hwids are reset successfully");
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> ResetAllHwidByAppId(int applicationId, int userId, string securityKey)
        {

            var businessConditions = BusinessRules.Run(
                await _authService.CheckUserSecurityKeyValid(userId, securityKey), await CheckIfApplicationOwnerTrue(applicationId, userId));

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }

            var allKeys = await _keyLicenseDal.GetAll(k => k.ApplicationId == applicationId);
            foreach (var key in allKeys)
            {
                key.Hwid = null;
                await _keyLicenseDal.Update(key);
            }

            var log = new Log { Success = true, Message = $"All hwid reset successfully.", Date = DateTime.Now, OwnerId = userId };
            await _logService.Add(log);
            return new SuccessResult("All hwids are reset successfully");
        }



        private async Task<IResult> CheckKeyAndHwidIsValid(string keyLicense, string hwid, DateTime? expirationDate, string requestIp)
        {
            var getKey = await _keyLicenseDal.Get(k => k.AuthKey == keyLicense);
            if (getKey == null)
            {
                return new ErrorResult("Key not found");
            }

            if (String.IsNullOrEmpty(hwid))
            {
                return new ErrorResult("Hwid not valid!");
            }

            var getApplication = await _applicationDal.Get(a => a.Id == getKey.ApplicationId);

            if (getApplication == null)
            {
                return new ErrorResult("Application not found");
            }
            if (getApplication.Status == false)
            {
                return new ErrorResult("Application is not active!");
            }

            var log = new Log { OwnerId = getKey.OwnerId, Date = DateTime.Now, Message = $"Key error  {keyLicense} and IP {requestIp}" };

            if (getKey is { IsOwned: true })
            {
                if (getKey.Hwid != hwid)
                {
                    log.Message = $"Wrong hwid attempt {keyLicense}, request hwid: {hwid}";
                    log.Success = false;
                    await _logService.Add(log);
                    return new ErrorResult("Key is occupied from another user!");
                }

            }

            if (expirationDate <= DateTime.Now)
            {
                log.Message = $"Expired key attempt {keyLicense}";
                log.Success = false;
                await _logService.Add(log);
                return new ErrorResult("Key expired!");
            }


            return new SuccessResult();
        }

        [SecuredOperations("admin,reseller,localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        public async Task<IResult> CreateLicenseKey(int keyEnd, int applicationId, int requestId
            , string securityKey)
        {
            var user = await _userDal.Get(u => u.Id == requestId);

            if (user == null)
            {
                return new ErrorResult("User not found");
            }

            var getClaims = await _userDal.GetClaims(user);

            foreach (var claim in getClaims)
            {
                switch (claim.Name)
                {
                    case "admin":
                        return await CreateAdminLicenseKey(keyEnd, applicationId, requestId, securityKey);
                    case "reseller":
                        return await CreateAdminLicenseKey(keyEnd, applicationId, requestId, securityKey);
                    case "localseller":
                        return await CreateLocalSellerLicenseKey(keyEnd, requestId, securityKey);
                    default:
                        return new ErrorResult("User not found to create key!");
                }
            }

            return new SuccessResult("Key successfully created!");
        }

        [SecuredOperations("localseller")]
        [CacheRemoveAspect(("IKeyLicenseService.Get"))]
        private async Task<IResult> CreateLocalSellerLicenseKey(int keyEnd, int requestId, string securityKey)
        {
            var user = await _userDal.Get(u => u.Id == requestId);

            if (user == null) return new ErrorResult("User not found to create key!");

            var checkConditions =
                BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(requestId, securityKey),
                    await CheckUserBalance(user, keyEnd));

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }

            var getApplicationId = await _panelDal.Get(p => p.PanelSellerId == requestId);

            KeyLicense keyLicense = new();
            keyLicense.AuthKey = FormatLicenseKey(GetMd5Sum(""));
            keyLicense.ExpirationDate = keyEnd switch
            {
                1 => DateTime.Now.AddDays(1),
                2 => DateTime.Now.AddDays(7),
                _ => DateTime.Now.AddMonths(1)
            };
            keyLicense.OwnerId = user.Id;
            keyLicense.ApplicationId = getApplicationId!.ApplicationId;
            await Add(keyLicense, requestId, securityKey);

            var getPanel = await _panelDal.Get(p => p.PanelSellerId == requestId);
            if (getPanel != null)
            {
                getPanel.CreatedLicense += 1;
                await _panelDal.Update(getPanel);
            }

            var log = new Log { Success = true, Message = $"{keyLicense.AuthKey} created successfully.", Date = DateTime.Now, OwnerId = keyLicense.OwnerId };
            await _logService.Add(log);
            return new SuccessResult("Key successfully created!");
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect("IKeyLicenseService.Get")]
        private async Task<IResult> CreateAdminLicenseKey(int keyEnd, int applicationId, int requestId, string securityKey)
        {
            var user = await _userDal.Get(u => u.Id == requestId);

            if (user == null) return new ErrorResult("User not found to create key!");

            var checkConditions =
                BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(requestId, securityKey),
                    await CheckUserBalance(user, keyEnd), await CheckIfApplicationOwnerTrue(applicationId, requestId)
    );

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }


            KeyLicense keyLicense = new();
            keyLicense.AuthKey = FormatLicenseKey(GetMd5Sum(""));
            keyLicense.ExpirationDate = keyEnd switch
            {
                1 => DateTime.Now.AddDays(1),
                2 => DateTime.Now.AddDays(7),
                _ => DateTime.Now.AddMonths(1)
            };
            keyLicense.OwnerId = user.Id;
            keyLicense.ApplicationId = applicationId;
            await Add(keyLicense, requestId, securityKey);

            var getPanel = await _panelDal.Get(p => p.PanelSellerId == requestId);
            if (getPanel != null)
            {
                getPanel.CreatedLicense += 1;
                await _panelDal.Update(getPanel);
            }

            var log = new Log { Success = true, Message = $"{keyLicense.AuthKey} created successfully.", Date = DateTime.Now, OwnerId = keyLicense.OwnerId };
            await _logService.Add(log);
            return new SuccessResult("Key successfully created!");
        }

        private async Task<IResult> CheckIfApplicationOwnerTrue(int applicationId, int requestId)
        {
            var application = await _applicationDal.Get(a => a.Id == applicationId);
            if (application == null) return new ErrorResult("Application not found!");
            if (application.OwnerId != requestId) return new ErrorResult("You are not owner of this application!");
            return new SuccessResult();
        }

        private async Task<IResult> CheckUserBalance(User user, int keyEnd)
        {
            //Here can be changed, example store prices in table and get from sql and calculate here
            switch (keyEnd)
            {
                case 1 when user.Balance - 3 >= 0:
                    user.Balance = user.Balance - 10;
                    await _userDal.Update(user);
                    return new SuccessResult();
                case 2 when user.Balance - 15 >= 0:
                    user.Balance = user.Balance - 20;
                    await _userDal.Update(user);
                    return new SuccessResult();
                case 3 when user.Balance - 30 >= 0:
                    user.Balance = user.Balance - 30;
                    await _userDal.Update(user);
                    return new SuccessResult();
                default:
                    return new ErrorResult("Don't enough balance for create a new key!");
            }
        }


        static string GetMd5Sum(string productIdentifier)
        {
            Encoder enc = Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[productIdentifier.Length * 2];
            enc.GetBytes(productIdentifier.ToCharArray(), 0, productIdentifier.Length, unicodeText, 0, true);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new();
            foreach (var t in result)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        static string FormatLicenseKey(string productIdentifier)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[28];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            productIdentifier = finalString;

            productIdentifier = productIdentifier.Substring(0, 20).ToUpper();
            char[] serialArray = productIdentifier.ToCharArray();
            StringBuilder licenseKey = new();

            int j = 0;
            for (int i = 0; i < 20; i++)
            {
                for (j = i; j < 4 + i; j++)
                {
                    licenseKey.Append(serialArray[j]);
                }

                if (j == 20)
                {
                    break;
                }

                i = (j) - 1;
                licenseKey.Append("-");
            }

            return licenseKey.ToString();
        }
    }
}