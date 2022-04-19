using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface ILogService
    {
        Task<IDataResult<List<Log>>> GetAll(int userId);
        Task<IDataResult<Log>> GetById(int logId);
        Task<IResult> Add(Log log);
        Task<IResult> Delete(int logId);
    }
}