using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Security.JWT.Concrete;
using Entities.Dto;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<User>> Register(UserRegisterDto userRegisterDto, string password);
        Task<IDataResult<User>> Login(UserLoginDto userLoginDto);
        Task<IDataResult<AccessToken>> CreateAccessToken(User user);
        Task<IResult> UserOwnControl(int id, string securityKey);
        Task<IResult> CheckSecurityKeyOutdated(int id);
        Task<IResult> CheckUserSecurityKeyValid(int id, string securityKey);
    }
}