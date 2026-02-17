using MobileAPI.DTOs.Common;
using MobileAPI.Models;

namespace MobileAPI.Interface
{
    public interface IOTPRepository
    {
        Task InsertAsync(OtpEntity otp);
        Task<OtpEntity?> GetValidOtpAsync(string mobile);
        Task MarkUsedAsync(int id, string status);

    }

}
