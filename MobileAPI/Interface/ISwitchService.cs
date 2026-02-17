namespace MobileAPI.Interface
{
    public interface ISwitchService
    {
        Task<TResponse> SendAsync<TResponse>(string endpoint, string version, string txnId, object requestObject);
    }
}
