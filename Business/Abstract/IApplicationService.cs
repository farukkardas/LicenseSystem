using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IApplicationService
    {
        Task<IDataResult<List<Application>>> GetAll();
        Task<IDataResult<List<Application>>> GetById(int userId,string securityKey);
        Task<IResult> Add(int userId,string securityKey,Application application);
        Task<IResult> Update(int userId,string securityKey,Application application);
        Task<IResult> Delete(int userId,string securityKey,int id);
    }
}