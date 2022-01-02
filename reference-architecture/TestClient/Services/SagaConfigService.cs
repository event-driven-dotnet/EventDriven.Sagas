using System.Net.Http.Json;
using EventDriven.Sagas.Abstractions.Repositories;
using TestClient.Configuration;

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
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(id.ToString()));
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<SagaConfiguration?> PostSagaConfiguration(SagaConfiguration sagaConfig)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PostAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<SagaConfiguration?> PutSagaConfiguration(SagaConfiguration sagaConfig)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PutAsJsonAsync("", sagaConfig);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<SagaConfiguration>();
        return content;
    }

    public async Task<int> DeleteSagaConfiguration(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .DeleteAsync(new Uri(id.ToString()));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<int>();
        return content;
    }
}