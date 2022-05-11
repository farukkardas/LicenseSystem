using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Business.Concrete
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationDal _applicationDal;
        private readonly IAuthService _authService;
        private readonly IKeyLicenseDal _keyLicenseDal;
        private readonly IPanelDal _panelDal;
        private readonly IUserDal _userDal;
        private readonly IKeyLicenseService _keyLicenseService;
        public ApplicationService(IApplicationDal applicationDal, IAuthService authService, IKeyLicenseDal keyLicenseDal, IPanelDal panelDal, IUserDal userDal, IKeyLicenseService keyLicenseService)
        {
            _applicationDal = applicationDal;
            _authService = authService;
            _keyLicenseDal = keyLicenseDal;
            _panelDal = panelDal;
            _userDal = userDal;
            _keyLicenseService = keyLicenseService;
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect("IApplicationService.Get")]
        public async Task<IResult> Add(int userId, string securityKey, Application application)
        {
            var checkSk = await _authService.CheckUserSecurityKeyValid(userId, securityKey);

            if (!checkSk.Success)
            {
                return new ErrorDataResult<List<Application>>(checkSk.Message);
            }

            application.Status = true;
            application.CreationTime = DateTime.Now;
            application.OwnerId = userId;
            application.SecretKey = await GenerateNewSecretKey();
            application.DailyPrice = 3;
            application.WeeklyPrice = 20;
            application.MonthlyPrice = 30;
            await _applicationDal.Add(application);

            return new SuccessResult("Application successfully created.");
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect("IPanelService.Get")]
        [CacheRemoveAspect("IApplicationService.Get")]
        [CacheRemoveAspect("IKeyLicenseService.Get")]    
      //  [TransactionScopeAspect]
        public async Task<IResult> Delete(int userId, string securityKey, int id)
        {
            var application = await _applicationDal.Get(app => app.Id == id);

            if (application == null)
            {
                return new ErrorResult("Application not found for delete.");
            }

            if (application.OwnerId != userId)
            {
                return new ErrorResult("You can not delete this application.");
            }

            // Delete all keys and panels in this application
            var allKeys = await _keyLicenseDal.GetAll(k => k.ApplicationId == id);
            foreach (var key in allKeys)
            {
                await _keyLicenseDal.Delete(key);
            }

            var allPanels = await _panelDal.GetAll(p => p.ApplicationId == id);
            foreach (var panel in allPanels)
            {
                var user = await _userDal.Get(u => u.Id == panel.PanelSellerId);
                if (user != null)
                {
                    await _userDal.Delete(user);
                }
                await _panelDal.Delete(panel);
            }

            await _applicationDal.Delete(application);

            return new SuccessResult("Application successfully deleted.");
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect("IApplicationService.Get")]
        public async Task<IResult> DisableApplication(int userId, string securityKey, int applicationId)
        {
            var application = await _applicationDal.Get(app => app.Id == applicationId);
            if (application == null)
            {
                return new ErrorResult("Application not found for disable.");
            }

            if (application.OwnerId != userId)
            {
                return new ErrorResult("You can not disable this application.");
            }

            application.Status = application.Status == true ? false : true;
            await _applicationDal.Update(application);
            string message = application.Status == true ? "Application successfully enabled." : "Application successfully disabled.";
            return new SuccessResult(message);
        }


        [SecuredOperations("admin,reseller")]
        [CacheAspect]
        public async Task<IDataResult<List<Application>>> GetAll()
        {
            var result = await _applicationDal.GetAll();

            return new SuccessDataResult<List<Application>>(result);
        }

        [SecuredOperations("admin,reseller")]
        [CacheAspect]
        public async Task<IDataResult<List<Application>>> GetById(int userId, string securityKey)
        {
            var checkSk = await _authService.CheckUserSecurityKeyValid(userId, securityKey);

            if (!checkSk.Success)
            {
                return new ErrorDataResult<List<Application>>(checkSk.Message);
            }

            var getUser = await _userDal.Get(u => u.Id == userId);

            if (getUser == null)
            {
                return new ErrorDataResult<List<Application>>("User not found.");
            }

            var getClaims = await _userDal.GetClaims(getUser);
            var applications = await _applicationDal.GetAll(app => app.OwnerId == userId);
            foreach (var claim in getClaims)
            {
                switch (claim.Name)
                {
                    case "admin":
                        return new SuccessDataResult<List<Application>>(applications);
                    case "reseller":
                        return new SuccessDataResult<List<Application>>(applications);
                    case "localseller":
                        var getPanel = await _panelDal.Get(p => p.PanelSellerId == userId);
                        applications = await _applicationDal.GetAll(app => app.Id == getPanel.ApplicationId);
                        return new SuccessDataResult<List<Application>>(applications);
                    default:
                        return new SuccessDataResult<List<Application>>("User not found to create key!");
                }
            }

            var result = await _applicationDal.GetAll(app => app.OwnerId == userId);

            return new SuccessDataResult<List<Application>>(result);
        }

        [CacheRemoveAspect("IApplicationService.Get")]
        [SecuredOperations("admin,reseller")]
        public async Task<IResult> SetApplicationPrices(SetApplicationPrices applicationPrices, int userId, string securityKey)
        {
            var checkConditions =
                  BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey),
                      await _keyLicenseService.CheckIfApplicationOwnerTrue(applicationPrices.ApplicationId, userId));

            if (checkConditions != null)
            {
                return new ErrorResult(checkConditions.Message);
            }

            var getApplication = await _applicationDal.Get(app => app.Id == applicationPrices.ApplicationId);

            getApplication.DailyPrice = applicationPrices.DailyPrice;
            getApplication.WeeklyPrice = applicationPrices.WeeklyPrice;
            getApplication.MonthlyPrice = applicationPrices.MonthlyPrice;
            await _applicationDal.Update(getApplication);

            return new SuccessResult("Application prices successfully set.");
        }

        [SecuredOperations("admin,reseller")]
        [CacheAspect]
        public async Task<IResult> Update(int userId, string securityKey, Application application)
        {
            var checkSk = await _authService.CheckUserSecurityKeyValid(userId, securityKey);

            if (!checkSk.Success)
            {
                return new ErrorDataResult<List<Application>>(checkSk.Message);
            }

            await _applicationDal.Update(application);

            return new SuccessResult("Application successfully updated.");
        }

        private async Task<string> GenerateNewSecretKey()
        {
            return await Task.Run(() =>
            {

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[50];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

              

                return finalString;
            });


        }
    }
}