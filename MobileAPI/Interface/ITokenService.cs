namespace MobileAPI.Interface
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string mobile);
    }

}
