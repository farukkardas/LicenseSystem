using System;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Business.Validation;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT.Abstract;
using Core.Utilities.Security.JWT.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using Entities.Dto;

namespace Business.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUserDal _userDal;
        private readonly ITokenHelper _tokenHelper;
        private readonly IPanelDal _panelDal;
        private readonly ILogService _logService;

        public AuthService(IUserDal userDal, ITokenHelper tokenHelper, IPanelDal panelDal, ILogService logService)
        {
            _userDal = userDal;
            _tokenHelper = tokenHelper;
            _panelDal = panelDal;
            _logService = logService;
        }

        [SecuredOperations("admin")]
        [ValidationAspect(typeof(RegisterValidator))]
        public async Task<IDataResult<User>> Register(UserRegisterDto userRegisterDto, string password)
        {
            HashingHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var user = new User
            {
                Email = userRegisterDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };

            await _userDal.Add(user);
            await _userDal.SetClaims(user.Id);
            await GenerateRandomSecurityKey(user);

            return new SuccessDataResult<User>(user, $"{user.Email} successfully created!");
        }


        public async Task<IDataResult<User>> Login(UserLoginDto userLoginDto)
        {
            var userToCheck = await _userDal.Get(u => u.Email == userLoginDto.Email);


            if (userToCheck == null)
            {
                return new ErrorDataResult<User>("User not found!");
            }

            await GenerateRandomSecurityKey(userToCheck);

            IResult result = BusinessRules.Run(await CheckUserStatus(userLoginDto.Email));

            if (result != null)
            {
                return new ErrorDataResult<User>($"{result.Message}");
            }

            if (!HashingHelper.VerifyPasswordHash(userLoginDto.Password, userToCheck.PasswordHash,
                userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Wrong password!");
            }

            var log = new Log
            {
                Success = true, Date = DateTime.Now, Message = $"{userToCheck.Email} logged successfully"
            };

            await _logService.Add(log);
            return new SuccessDataResult<User>(userToCheck);
        }

        public async Task<IDataResult<AccessToken>> CreateAccessToken(User user)
        {
            var claims = await _userDal.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);

            return new SuccessDataResult<AccessToken>(accessToken, "Successful login!");
        }

        public Task<IResult> UserOwnControl(int id, string securityKey)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IResult> CheckSecurityKeyOutdated(int id)
        {
            var result = await _userDal.Get(u => u.Id == id);

            if (result == null)
            {
                return new ErrorResult("User not found, logout");
            }

            if (result.SecurityKeyExpiration < DateTime.Now)
            {
                return new ErrorResult("Security key outdated");
            }

            return new SuccessResult("Security key is up to date.");
        }

        public async Task<IResult> CheckUserSecurityKeyValid(int requestId, string securityKey)
        {
            var user = await _userDal.Get(u => u.Id == requestId);

            if (user?.SecurityKey != securityKey)
            {
                return new ErrorResult("Your security key is not valid please login again.");
            }

            return new SuccessResult();
        }


        private async Task<IResult> CheckUserStatus(string email)
        {
            var user = await _userDal.Get(u => u.Email == email);
            var panel = await _panelDal.Get(p => user != null && p.PanelSellerId == user.Id);
            if (user is {Status: false})
            {
                return new ErrorResult("Your account is disabled.");
            }

            if (panel?.IsActive == false)
            {
                return new ErrorResult("Your panel is disabled!");
            }

            return new SuccessResult();
        }

        private async Task GenerateRandomSecurityKey(User user)
        {
            await using var context = new LicenseSystemContext();

            if (user == null || user.SecurityKeyExpiration > DateTime.Now)
            {
                return;
            }

            user.SecurityKey = await RandomSecurityKey();
            user.SecurityKeyExpiration = DateTime.Now.AddDays(1);

            await _userDal.Update(user);

            await context.SaveChangesAsync();
        }

        private async Task<string> RandomSecurityKey()
        {
            return await Task.Run((() =>
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[100];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                return finalString;
            }));
        }
    }
}