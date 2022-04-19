using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Dto;

namespace DataAccess.Concrete
{
    public class EfUserDal : EfEntityRepositoryBase<User, LicenseSystemContext>, IUserDal
    {
        public async Task SetClaims(int id)
        {
            await using var context = new LicenseSystemContext();

            UserOperationClaim userOperationClaim = new() { UserId = id, OperationClaimId = 3 };


            context.UserOperationClaims?.Add(userOperationClaim);

            await context.SaveChangesAsync();
        }

        public async Task<List<OperationClaim>> GetClaims(User user)
        {
            await using var context = new LicenseSystemContext();
            var result = from operationClaim in context.OperationClaims
                         join userOperationClaim in context.UserOperationClaims
                             on operationClaim.Id equals userOperationClaim.OperationClaimId
                         where userOperationClaim.UserId == user.Id
                         select new OperationClaim
                         {
                             Id = operationClaim.Id,
                             Name = operationClaim.Name
                         };
            return result.ToList();
        }

        public async Task<UserDetailsDto> GetUserDetails(int userId)
        {
            await using var context = new LicenseSystemContext();

            var result = from users in context.Users
                         where users.Id == userId
                         select new UserDetailsDto
                         {
                             UserId = userId,
                             LicensesCount = context.KeyLicenses!.Where(k => k.OwnerId == userId).Count(),
                             PanelsCount = context.Panels!.Where(p => p.PanelOwnerId == userId).Count(),
                             UsedKeys = context.KeyLicenses!.Where(k => k.OwnerId == userId && k.IsOwned == true).Count(),
                             UnusedKeys = context.KeyLicenses!.Where(k => k.OwnerId == userId && k.IsOwned == false).Count()
                         };
            return result.First();
        }
    }
}