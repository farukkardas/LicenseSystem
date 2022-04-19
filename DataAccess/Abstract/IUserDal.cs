using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess.Abstract;
using Core.Entities.Concrete;
using Entities.Dto;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
        Task SetClaims(int userId);
        Task<List<OperationClaim>> GetClaims(User user);
        Task<UserDetailsDto> GetUserDetails(int userId);
    }
}