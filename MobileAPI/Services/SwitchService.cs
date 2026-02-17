using System.Text;
using Azure.Core;
using MobileAPI.Interface;
using MobileAPI.Models;
using Newtonsoft.Json;

public class SwitchService : ISwitchService
{
    private readonly HttpClient _client;

    public SwitchService(HttpClient client)
    {
        _client = client;
    }

    public async Task<TResponse> SendAsync<TResponse>(string endpoint,string version,string txnId,object requestObject)
    {
        try
        {
            var json = JsonConvert.SerializeObject(requestObject);

            var apiUrl = $"{endpoint}/{version}/urn:txnid:{txnId}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(apiUrl, content);

            var respJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Switch API failed | URL: {_client.BaseAddress}{apiUrl} | " +
                    $"Status: {response.StatusCode} | Response: {respJson}");
            }

            var result = JsonConvert.DeserializeObject<TResponse>(respJson);

          
            return result;
        }
        catch (HttpRequestException ex)
        {
            
            throw new Exception("Switch connection failed (host unreachable or server down)", ex);
        }
        catch (TaskCanceledException ex)
        {
            
            throw new Exception("Switch request timeout", ex);
        }
        catch (JsonException ex)
        {
            
            throw new Exception("Switch response parsing failed", ex);
        }
        catch (Exception ex)
        {
            
            throw new Exception("Unexpected error while calling switch", ex);
        }
    }







}
