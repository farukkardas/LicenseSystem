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
        public ApplicationService(IApplicationDal applicationDal, IAuthService authService)
        {
            _applicationDal = applicationDal;
            _authService = authService;
        }

        [SecuredOperations("admin,reseller,localseller")]
        public async Task<IResult> Add(int userId,string securityKey,Application application)
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

            return new SuccessResult("Key successfully created.");
        }

        [SecuredOperations("admin,reseller,localseller")]
        public async Task<IResult> Delete(int userId,string securityKey,int id)
        {
            var application = await _applicationDal.Get(app => app.Id == id);

            if (application == null)
            {
                return new ErrorResult("Application not found for delete.");
            }

            await _applicationDal.Delete(application);

            return new SuccessResult("Key successfully deleted.");
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

        public async Task<IResult> Update(int userId,string securityKey,Application application)
        {
             var checkSk = await _authService.CheckUserSecurityKeyValid(userId, securityKey);

            if (!checkSk.Success)
            {
                return new ErrorDataResult<List<Application>>(checkSk.Message);
            }

            await _applicationDal.Update(application);

            return new SuccessResult("Key successfully updated.");
        }
    }
}