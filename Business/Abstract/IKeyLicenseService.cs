using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IKeyLicenseService
    {
        Task<IDataResult<List<KeyLicense>>> GetAll();
        Task<IDataResult<KeyLicense>> GetById(string authKey);
        Task<IResult> Add(KeyLicense key, int id, string securityKey);
        Task<IResult> Delete(int keyId, int userId, string securityKey);
        Task<IResult> CreateLicenseKey(int keyEnd,int applicationId, int userId, string securityKey);
        Task<IDataResult<List<KeyLicense>>> GetLicenses(int id, string securityKey);
        Task<IDataResult<List<KeyLicense>>> GetLicensesByAppId(int applicationId,int userId, string securityKey);
        Task<IResult> ResetHwid(int keyId, int userId, string securityKey);
        Task<IResult> CheckLicense(string keyLicense, string hwid,string requestIp);
        Task<IResult> DeleteAllKeys(int userId, string securityKey);
        Task<IResult> DeleteAllKeysByAppId(int appId,int userId, string securityKey);
        Task<IResult> ResetAllHwid(int userId, string securityKey);
        Task<IResult> ResetAllHwidByAppId(int applicationId,int userId, string securityKey);
        Task<IResult> ExtendAllKeys(int timeSelection,int dateOption,int applicationId,int userId, string securityKey);
        Task<IResult> ExtendSingleKey(int timeSelection,int dateOption,int keyId,int userId,string securityKey);
    }
}