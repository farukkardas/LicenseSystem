using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class LogService : ILogService
    {
        private readonly ILogDal _logDal;
        private readonly IUserDal _userDal;

        public LogService(ILogDal logDal, IUserDal userDal)
        {
            _logDal = logDal;
            _userDal = userDal;
        }


        [SecuredOperations("admin,reseller,localseller")]
        public async Task<IDataResult<List<Log>>> GetAll(int userId)
        {
            var user = await _userDal.Get(u => u.Id == userId);
            var result = await _logDal.GetAll();
            if (user != null)
            {
                var userRole = await _userDal.GetClaims(user);
                foreach (var role in userRole)
                {
                    result = role.Name switch
                    {
                        "admin" => await _logDal.GetAll(),
                        "reseller" => await _logDal.GetAll(l => l.OwnerId == userId),
                        "localseller" => await _logDal.GetAll(l => l.OwnerId == userId),
                        _ => result
                    };
                }
            }


            return new SuccessDataResult<List<Log>>(result);
        }

        public async Task<IDataResult<Log>> GetById(int logId)
        {
            var result = await _logDal.Get(l => l.Id == logId);

            return new SuccessDataResult<Log>(result);
        }

        public async Task<IResult> Add(Log log)
        {
            await _logDal.Add(log);

            return new SuccessResult("Log added");
        }

        public async Task<IResult> Delete(int logId)
        {
            var log = await _logDal.Get(l => l.Id == logId);

            if (log != null) await _logDal.Delete(log);

            return new SuccessResult("Log deleted!");
        }
    }
}