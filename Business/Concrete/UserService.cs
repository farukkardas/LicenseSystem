using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Business.Validation;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Dto;

namespace Business.Concrete
{

    public class UserService : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IAuthService _authService;

        public UserService(IUserDal userDal, IAuthService authService)
        {
            _userDal = userDal;
            _authService = authService;
        }

        [SecuredOperations("admin")]
        public async Task<IDataResult<List<User>>> GetAll()
        {
            var result = await _userDal.GetAll();

            return new SuccessDataResult<List<User>>(result);
        }

        [SecuredOperations("admin")]
        public async Task<IDataResult<User>> GetById(int userId)
        {
            var result = await _userDal.Get(u => u.Id == userId);

            return new SuccessDataResult<User>(result);
        }


        [ValidationAspect(typeof(UserValidator))]
        [SecuredOperations("admin,reseller")]
        public async Task<IResult> Add(User user)
        {
            await _userDal.Add(user);

            return new SuccessResult("User added successfully!");
        }
        [SecuredOperations("admin")]
        public async Task<IResult> Delete(int userId)
        {
            var user = await _userDal.Get(u => u.Id == userId);
            if (user == null) return new ErrorResult("User not found to delete!");
            await _userDal.Delete(user);
            return new SuccessResult("User successfully deleted!");
        }

        public async Task<User?> GetByMail(string email)
        {
            return await _userDal.Get(u => u.Email == email);
        }

        public  async  Task<List<OperationClaim>> GetClaims(User user)
        {
            return await _userDal.GetClaims(user);
        }
        [SecuredOperations("admin,localseller,reseller")]
        public async Task<IDataResult<User>> GetUserDetails(int userId, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey));

            if (checkConditions != null)
            {
                return new ErrorDataResult<User>(checkConditions.Message);

            }
            
            var result = await GetById(userId);
            return new SuccessDataResult<User>(result.Data);
        }

        public async Task<IDataResult<UserDetailsDto>> GetUserInformations(int userId, string securityKey)
        {
            var checkConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey));

            if (checkConditions != null)
            {
                return new ErrorDataResult<UserDetailsDto>(checkConditions.Message);
            }

            var result = await _userDal.GetUserDetails(userId);

            return new SuccessDataResult<UserDetailsDto>(result);
        }
    }
}