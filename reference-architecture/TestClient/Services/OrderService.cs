﻿using System.Net;
using System.Net.Http.Json;
using OrderService.Domain.OrderAggregate;
using TestClient.Configuration;

namespace TestClient.Services;

public class OrderService
{
    private readonly Uri _baseAddress;
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderService(
        IHttpClientFactory httpClientFactory,
        OrderServiceSettings settings)
    {
        _httpClientFactory = httpClientFactory;
        _baseAddress = new Uri(settings.ServiceUri);
    }

    public async Task<OrderState?> GetOrderState(Guid id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(id.ToString()));
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        OrderState? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<OrderState>();
        return content;
    }

    public async Task<Order?> CreateOrder(Order order)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var response = await httpClient
            .PostAsJsonAsync("", order);
        response.EnsureSuccessStatusCode();
        Order? content = null;
        if (response.StatusCode != HttpStatusCode.NoContent)
            content = await response.Content.ReadFromJsonAsync<Order>();
        return content;
    }
}