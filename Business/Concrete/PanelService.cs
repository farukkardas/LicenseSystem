using System;
using Business.Abstract;
using Business.BusinessAspects;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Transaction;
using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using Entities.Dto;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Autofac.Caching;

namespace Business.Concrete
{
    public class PanelService : IPanelService
    {
        private readonly IPanelDal _panelDal;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserDal _userDal;

        public PanelService(IPanelDal panelDal, IAuthService authService, IUserService userService, IUserDal userDal)
        {
            _panelDal = panelDal;
            _authService = authService;
            _userService = userService;
            _userDal = userDal;
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect(("IPanelService.Get"))]
        public async Task<IResult> Add(Panel panel)
        {
            await _panelDal.Add(panel);
            return new SuccessResult("Panel created successfully!");
        }

        [SecuredOperations("admin,reseller")]
        [CacheRemoveAspect(("IPanelService.Get"))]
        public async Task<IResult> Delete(int panelId)
        {
            var panel = await _panelDal.Get(p => p.Id == panelId);
            if (panel == null)
                return new ErrorResult("Error when deleting panel!");

            await _panelDal.Delete(panel);
            return new SuccessResult("Panel deleted successfully");
        }



        [SecuredOperations("admin,reseller")]
        [ValidationAspect(typeof(PanelValidator))]
        [TransactionScopeAspect]
        [CacheRemoveAspect(("IPanelService.Get"))]
        public async Task<IResult> CreateNewPanel(PanelRegisterDto panelRegisterDto, int id, string securityKey)
        {
            var businessConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(id, securityKey),
                await CheckBalanceEnough(id, panelRegisterDto.Balance));

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }

            HashingHelper.CreatePasswordHash(panelRegisterDto.PanelPassword, out var passwordHash,
                out var passwordSalt);
            var user = new User
            {
                Email = panelRegisterDto.PanelMail,
                Balance = panelRegisterDto.Balance,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true,
            };
            await _userService.Add(user);
            await _userDal.SetClaims(user.Id);

            Panel panel = new()
            {
                PanelOwnerId = id,
                PanelSellerId = user.Id,
                Balance = panelRegisterDto.Balance,
                ApplicationId = panelRegisterDto.ApplicationId,
                IsActive = true
            };
            await Add(panel);

            var ownerUser = await _userDal.Get(u => u.Id == id);
            if (ownerUser != null)
            {
                ownerUser.Balance -= panelRegisterDto.Balance;
                await _userDal.Update(ownerUser);
            }

            return new SuccessResult("Panel created successfully!");
        }

        [CacheRemoveAspect(("IPanelService.Get"))]
        public async Task<IResult> DisablePanel(int panelId, int userId, string securityKey)
        {
            var businessConditions = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey),
                await CheckOwnPanel(panelId, userId));

            if (businessConditions != null)
            {
                return new ErrorResult(businessConditions.Message);
            }

            var panel = await _panelDal.Get(p => p.Id == panelId);
            if (panel != null) panel.IsActive = !panel.IsActive;
            await _panelDal.Update(panel);
            string returnMessage = panel?.IsActive == true ? "Panel enabled!" : "Panel disabled!";
            return new SuccessResult(returnMessage);
        }

        private async Task<IResult> CheckOwnPanel(int panelId, int userId)
        {
            var panel = await _panelDal.Get(p => p.PanelOwnerId == userId && p.Id == panelId);
            if (panel != null && panel.PanelOwnerId != userId)
            {
                return new ErrorResult("You cant disable this panel!");
            }

            return new SuccessResult();
        }

        private async Task<IResult> CheckBalanceEnough(int id, double balance)
        {
            var user = await _userDal.Get(u => u.Id == id);
            if (user == null)
            {
                return new ErrorResult("User not found for check balance!");
            }

            if (user.Balance - balance < 0 || balance > user.Balance)
            {
                return new ErrorResult("Balance not enough for create panel!");
            }

            if (balance < 100)
            {
                return new ErrorResult("Minimum panel price must be greater than 100!");
            }

            return new SuccessResult();
        }

        [SecuredOperations("admin,reseller")]
        [CacheAspect(60)]
        public async Task<IDataResult<List<Panel>>> GetAll()
        {
            var result = await _panelDal.GetAll();
            return new SuccessDataResult<List<Panel>>(result);
        }

        [SecuredOperations("admin,reseller")]
        [CacheAspect(60)]
        public async Task<IDataResult<List<PanelInfoDto>>> GetUserPanels(int userId, string securityKey)
        {
            var condition = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey));

            if (condition != null)
            {
                return new ErrorDataResult<List<PanelInfoDto>>(condition.Message);
            }

            var result = await _panelDal.GetPanelInformation(p => p.PanelOwnerId == userId);
            return new SuccessDataResult<List<PanelInfoDto>>(result);
        }

        [SecuredOperations("admin,reseller")]
        [CacheAspect(60)]
        public async Task<IDataResult<List<PanelInfoDto>>> GetPanelsByUserId(int userId, string securityKey, int applicationId)
        {
            var condition = BusinessRules.Run(await _authService.CheckUserSecurityKeyValid(userId, securityKey)
               );

            if (condition != null)
            {
                return new ErrorDataResult<List<PanelInfoDto>>(condition.Message);
            }

            var result = await _panelDal.GetPanelInformation(p => p.PanelOwnerId == userId && p.ApplicationId == applicationId);
            return new SuccessDataResult<List<PanelInfoDto>>(result);
        }

    }
}