using System.Net;
using System.Net.Http.Json;
using EventDriven.Sagas.Configuration.Abstractions.DTO;

namespace SagaConfigCli;

public class SagaConfigService
{
    private readonly Uri _baseAddress;
    private readonly IHttpClientFactory _httpClientFactory;

    public SagaConfigService(
        IHttpClientFactory httpClientFactory,
        Uri serviceUri)
    {
        _httpClientFactory = httpClientFactory;
        _baseAddress = serviceUri;
    }

    public async Task<SagaConfigurationDto?> GetSagaConfiguration(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var request = new HttpRequestMessage(HttpMethod.Get, id.ToString());
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        SagaConfigurationDto? content = null;
        if (response.StatusCode != HttpStatusCode.NotFound && response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfigurationDto>();
        return content;
    }

    public async Task<SagaConfigurationDto?> UpsertSagaConfiguration(SagaConfigurationDto? sagaConfig)
    {
        if (sagaConfig == null) return null;
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

    public async Task<SagaConfigurationDto?> PostSagaConfiguration(SagaConfigurationDto? sagaConfig)
    {
        if (sagaConfig == null) return null;
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PostAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        SagaConfigurationDto? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfigurationDto>();
        return content;
    }

    public async Task<SagaConfigurationDto?> PutSagaConfiguration(SagaConfigurationDto? sagaConfig)
    {
        if (sagaConfig == null) return null;
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PutAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        SagaConfigurationDto? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<SagaConfigurationDto>();
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