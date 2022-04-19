using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Dto;

namespace Business.Abstract
{
    public interface IUserService 
    {
        Task<IDataResult<List<User>>> GetAll();
        Task<IDataResult<User>> GetById(int userId);
        Task<IResult> Add(User user);
        Task<IResult> Delete(int userId);
        Task<User?> GetByMail(string email);
        Task<List<OperationClaim>> GetClaims(User user);
        Task<IDataResult<User>> GetUserDetails(int userId, string securityKey);
        Task<IDataResult<UserDetailsDto>> GetUserInformations(int userId, string securityKey);
    }
}