using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationDal _applicationDal;
        private readonly IAuthService _authService;
        private readonly IKeyLicenseDal _keyLicenseDal;
        private readonly IPanelDal _panelDal;
        public ApplicationService(IApplicationDal applicationDal, IAuthService authService, IKeyLicenseDal keyLicenseDal, IPanelDal panelDal)
        {
            _applicationDal = applicationDal;
            _authService = authService;
            _keyLicenseDal = keyLicenseDal;
            _panelDal = panelDal;
        }

        [SecuredOperations("admin,reseller,localseller")]
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
            await _applicationDal.Add(application);

            return new SuccessResult("Application successfully created.");
        }

        [SecuredOperations("admin,reseller,localseller")]
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
                await _panelDal.Delete(panel);
            }

            await _applicationDal.Delete(application);

            return new SuccessResult("Application successfully deleted.");
        }

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


        [SecuredOperations("admin")]
        public async Task<IDataResult<List<Application>>> GetAll()
        {
            var result = await _applicationDal.GetAll();

            return new SuccessDataResult<List<Application>>(result);
        }

        [SecuredOperations("admin,reseller,localseller")]
        public async Task<IDataResult<List<Application>>> GetById(int userId, string securityKey)
        {
            var checkSk = await _authService.CheckUserSecurityKeyValid(userId, securityKey);

            if (!checkSk.Success)
            {
                return new ErrorDataResult<List<Application>>(checkSk.Message);
            }

            var result = await _applicationDal.GetAll(app => app.OwnerId == userId);

            return new SuccessDataResult<List<Application>>(result);
        }

        [SecuredOperations("admin,reseller,localseller")]
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


    }
}