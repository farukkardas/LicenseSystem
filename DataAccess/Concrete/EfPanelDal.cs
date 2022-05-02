using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfPanelDal : EfEntityRepositoryBase<Panel, LicenseSystemContext>, IPanelDal
    {
        public async Task<List<PanelInfoDto>> GetPanelInformation(Expression<Func<PanelInfoDto, bool>> filter = null)
        {
            await using var context = new LicenseSystemContext();
            var result = from panels in context.Panels
                         join users in context.Users!
                         on panels.PanelSellerId equals users.Id
                         select new PanelInfoDto
                         {
                             Id = panels.Id,
                             ApplicationId = panels.ApplicationId,
                             Balance = panels.Balance,
                             CreatedLicense = panels.CreatedLicense,
                             IsActive = panels.IsActive,
                             PanelMail = users.Email,
                             PanelOwnerId = panels.PanelOwnerId,
                             PanelSellerId = panels.PanelSellerId,
                         };
        
            return filter == null ? result.ToList() : result.Where(filter).ToList();
        }
    }
}
