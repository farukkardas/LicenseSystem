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
        Task<IResult> CreateLicenseKey(int keyEnd, int userId, string securityKey);
        Task<IDataResult<List<KeyLicense>>> GetLicenses(int id, string securityKey);
        Task<IResult> ResetHwid(int keyId, int userId, string securityKey);
        Task<IResult> CheckLicense(string keyLicense, string hwid);
        Task<IResult> DeleteAllKeys(int userId, string securityKey);
        Task<IResult> ResetAllHwid(int userId, string securityKey);
    }
}