using System.Text;
using MobileAPI.DTOs.Common;
using MobileAPI.Interface;
using Newtonsoft.Json;

public class AccountService : IAccountService
{
    private readonly HttpClient _client;

    public AccountService(HttpClient client)
    {
        _client = client;
    }

    public async Task<AcDetailsResponseDto?> GetDetailsAsync(string accountNo)
    {
        var response = await _client.PostAsJsonAsync("api/MobileBanking/AcDetails",
            new { accountNo });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AcDetailsResponseDto>();
    }

    public async Task <AccountListResponseDto?> GetAccountListAsync(string mobileNo)
    {
        var response = await _client.PostAsJsonAsync("api/MobileBanking/AcList",
            new { mobileNo });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AccountListResponseDto>();
    }
}

