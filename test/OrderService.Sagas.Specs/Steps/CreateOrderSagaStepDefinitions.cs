using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.Specs.Configuration;
using OrderService.Sagas.Specs.Helpers;
using OrderService.Sagas.Specs.Repositories;
using SagaConfigService.Repositories;
using Xunit;

namespace OrderService.Sagas.Specs.Steps;

[Binding]
public class CreateOrderSagaStepDefinitions
{
    private HttpResponseMessage Response { get; set; } = null!;
    private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public ISagaConfigDtoRepository SagaConfigRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public OrderServiceSpecsSettings ServiceSpecsSettings { get; }
    public HttpClient Client { get; }
    public JsonFilesRepository JsonFilesRepo { get; }
    public SagaConfigurationDto? SagaConfiguration { get; set; }
    public Customer? Customer { get; set; }
    public Order? Order { get; set; }

    public CreateOrderSagaStepDefinitions(
        OrderServiceSpecsSettings serviceSpecsSettings,
        HttpClient httpClient,
        ISagaConfigDtoRepository sagaConfigRepository,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        JsonFilesRepository jsonFilesRepo)
    {
        ServiceSpecsSettings = serviceSpecsSettings;
        Client = httpClient;
        SagaConfigRepository = sagaConfigRepository;
        CustomerRepository = customerRepository;
        OrderRepository = orderRepository;
        JsonFilesRepo = jsonFilesRepo;
    }

    [Given(@"a saga configuration has been created with '(.*)'")]
    public async Task GivenASagaConfigurationHasBeenCreatedWith(string file)
    {
        var sagaConfigJson = JsonFilesRepo.Files[file];
        SagaConfiguration = JsonSerializer.Deserialize<SagaConfigurationDto>(sagaConfigJson, JsonSerializerOptions);
        if (SagaConfiguration != null)
            await SagaConfigRepository.AddSagaConfigurationAsync(SagaConfiguration);
    }

    [Given(@"a customer has been created with '(.*)'")]
    public async Task GivenHasBeenCreated(string file)
    {
        var customerJson = JsonFilesRepo.Files[file];
        Customer = JsonSerializer.Deserialize<Customer>(customerJson, JsonSerializerOptions);
        if (Customer != null)
            await CustomerRepository.AddAsync(Customer);
    }

    [Given(@"the customer credit is (.*)")]
    public async Task GivenTheCustomerCreditIs(decimal amount)
    {
        if (Customer != null)
        {
            Customer.CreditAvailable = 5.0M;
            await CustomerRepository.UpdateAsync(Customer);
        }
    }

    [When(@"I make a POST request with '(.*)' to '(.*)'")]
    public async Task WhenIMakeApostRequestWithTo(string file, string endpoint)
    {
        var json = JsonFilesRepo.Files[file];
        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = await Client.PostAsync(endpoint, content);
    }

    [Then(@"the response status code is '(.*)'")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        var expected = (HttpStatusCode)statusCode;
        Assert.Equal(expected, Response.StatusCode);
    }

    [Then(@"the location header is '(.*)'")]
    public void ThenTheLocationHeaderIs(string location)
    {
        var fullLocation = location.Replace("id", ServiceSpecsSettings.OrderId.ToString());
        Assert.Equal(new Uri(fullLocation, UriKind.Relative), Response.Headers.Location);
    }

    [Then(@"the response entity should be '(.*)'")]
    public async Task ThenTheResponseEntityShouldBe(string file)
    {
        var json = JsonFilesRepo.Files[file];
        Order = JsonSerializer.Deserialize<Order>(json, JsonSerializerOptions);
        var actual = await Response.Content.ReadFromJsonAsync<Order>();
        Assert.Equal(Order, actual, new OrderComparer()!);
    }

    [Then(@"the customer credit should equal (.*)")]
    public async Task ThenTheCustomerCreditShouldEqual(decimal creditAvailable)
    {
        if (Customer == null) return;
        await Task.Delay(ServiceSpecsSettings.SagaCompletionTimeout);
        var customer = await CustomerRepository.GetAsync(Customer.Id);
        Assert.Equal(creditAvailable, customer?.CreditAvailable);
    }

    [Then(@"the order state should be '(.*)'")]
    public async Task ThenTheOrderStateShouldBe(OrderState state)
    {
        if (Order == null) return;
        await Task.Delay(ServiceSpecsSettings.SagaCompletionTimeout);
        var orderState = await OrderRepository.GetOrderStateAsync(Order.Id);
        Assert.Equal(state, orderState);
    }
}