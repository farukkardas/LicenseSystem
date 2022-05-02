using Core.DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Dto;
using System.Linq.Expressions;
using System;

namespace DataAccess.Abstract
{
    public interface IPanelDal : IEntityRepository<Panel>
    {
        Task<List<PanelInfoDto>> GetPanelInformation(Expression<Func<PanelInfoDto, bool>> filter = null);
    }
}
