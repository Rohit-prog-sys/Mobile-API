using MobileAPI.DTOs.Common;

namespace MobileAPI.Interface
{
    public interface IAccountService
    {
        Task<AcDetailsResponseDto?> GetDetailsAsync(string accountNo);

        Task<AccountListResponseDto?> GetAccountListAsync(string mobileNo);
    }

}
