using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Dto;

namespace Business.Abstract
{
    public interface IPanelService
    {
        Task<IDataResult<List<Panel>>> GetAll();
        Task<IDataResult<List<PanelInfoDto>>> GetUserPanels(int userId,string securityKey);
        Task<IResult> Add(Panel panel);
        Task<IResult> Delete(int panelId);
        Task<IResult> CreateNewPanel(PanelRegisterDto panelRegisterDto,int id, string securityKey);
        Task<IResult> DisablePanel(int panelId,int userId, string securityKey);
         Task<IDataResult<List<PanelInfoDto>>> GetPanelsByUserId(int userId, string securityKey,int applicationId);
    }
}
