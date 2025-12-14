using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace MesClient.Infrastructure.Api;

/// <summary>
/// API 클라이언트
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private string? _accessToken;

    public ApiClient(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// 액세스 토큰 설정
    /// </summary>
    public void SetAccessToken(string? token)
    {
        _accessToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// GET 요청
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("GET {Endpoint}", endpoint);
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "GET 요청 실패: {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// POST 요청
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("POST {Endpoint}", endpoint);
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "POST 요청 실패: {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// PUT 요청
    /// </summary>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("PUT {Endpoint}", endpoint);
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "PUT 요청 실패: {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// DELETE 요청
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("DELETE {Endpoint}", endpoint);
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DELETE 요청 실패: {Endpoint}", endpoint);
            throw;
        }
    }
}

/// <summary>
/// API 클라이언트 인터페이스
/// </summary>
public interface IApiClient
{
    void SetAccessToken(string? token);
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}
