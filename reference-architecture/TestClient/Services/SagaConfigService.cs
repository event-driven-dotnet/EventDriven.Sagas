using System.Net;
using System.Net.Http.Json;
using TestClient.Configuration;
using TestClient.DTO;

namespace TestClient.Services;

public class SagaConfigService
{
    private readonly Uri _baseAddress;
    private readonly IHttpClientFactory _httpClientFactory;

    public SagaConfigService(
        IHttpClientFactory httpClientFactory,
        SagaConfigServiceSettings settings)
    {
        _httpClientFactory = httpClientFactory;
        _baseAddress = new Uri(settings.ServiceUri);
    }

    public async Task<SagaConfiguration?> GetSagaConfiguration(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var request = new HttpRequestMessage(HttpMethod.Get, id.ToString());
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        SagaConfiguration? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<SagaConfiguration?> UpsertSagaConfiguration(SagaConfiguration sagaConfig)
    {
        var result = await GetSagaConfiguration(sagaConfig.Id);
        if (result == null)
            result = await PostSagaConfiguration(sagaConfig);
        else
        {
            sagaConfig.ETag = result.ETag;
            result = await PutSagaConfiguration(sagaConfig);
        }

        return result;
    }

    public async Task<SagaConfiguration?> PostSagaConfiguration(SagaConfiguration sagaConfig)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PostAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        SagaConfiguration? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<SagaConfiguration?> PutSagaConfiguration(SagaConfiguration sagaConfig)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PutAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        SagaConfiguration? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<int?> DeleteSagaConfiguration(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .DeleteAsync(id.ToString());
        response.EnsureSuccessStatusCode();
        int? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<int>();
        return content;
    }
}