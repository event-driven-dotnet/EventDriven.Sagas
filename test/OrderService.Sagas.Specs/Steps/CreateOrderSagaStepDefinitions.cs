using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using InventoryService.Domain.InventoryAggregate;
using InventoryService.Repositories;
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
    private List<HttpResponseMessage> Responses { get; set; } = new();
    private JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public ISagaConfigDtoRepository SagaConfigRepository { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IInventoryRepository InventoryRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public OrderServiceSpecsSettings ServiceSpecsSettings { get; }
    public HttpClient Client { get; }
    public JsonFilesRepository JsonFilesRepo { get; }
    public SagaConfigurationDto? SagaConfiguration { get; set; }
    public Customer? Customer { get; set; }
    public Inventory? Inventory { get; set; }
    public Order? Order { get; set; }
    public List<Order?> Orders { get; } = new();

    public CreateOrderSagaStepDefinitions(
        OrderServiceSpecsSettings serviceSpecsSettings,
        HttpClient httpClient,
        ISagaConfigDtoRepository sagaConfigRepository,
        ICustomerRepository customerRepository,
        IInventoryRepository inventoryRepository,
        IOrderRepository orderRepository,
        JsonFilesRepository jsonFilesRepo)
    {
        ServiceSpecsSettings = serviceSpecsSettings;
        Client = httpClient;
        SagaConfigRepository = sagaConfigRepository;
        CustomerRepository = customerRepository;
        InventoryRepository = inventoryRepository;
        OrderRepository = orderRepository;
        JsonFilesRepo = jsonFilesRepo;
    }

    [Given(@"a saga configuration has been created with '(.*)'")]
    public async Task GivenASagaConfigurationHasBeenCreatedWith(string file)
    {
        var sagaConfigJson = JsonFilesRepo.Files[file];
        SagaConfiguration = JsonSerializer.Deserialize<SagaConfigurationDto>(sagaConfigJson, JsonSerializerOptions);
        if (SagaConfiguration != null)
            await SagaConfigRepository.AddAsync(SagaConfiguration);
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
            Customer.CreditAvailable = amount;
            await CustomerRepository.UpdateAsync(Customer);
        }
    }

    [Given(@"inventory has been created with '(.*)'")]
    public async Task GivenProductsHaveBeenCreatedWith(string file)
    {
        var inventoryJson = JsonFilesRepo.Files[file];
        Inventory = JsonSerializer.Deserialize<Inventory>(inventoryJson, JsonSerializerOptions);
        if (Inventory != null)
        {
            await InventoryRepository.AddAsync(Inventory);
        }
    }

    [Given(@"the inventory quantity is (.*)")]
    public async Task GivenTheInventoryQuantityIs(int quantity)
    {
        if (Inventory != null)
        {
            Inventory.AmountAvailable = quantity;
            await InventoryRepository.UpdateAsync(Inventory);
        }
    }

    [When(@"I make a POST request with '(.*)' to '(.*)'")]
    public async Task WhenIMakeApostRequestWithTo(string file, string endpoint)
    {
        var json = JsonFilesRepo.Files[file];
        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = await Client.PostAsync(endpoint, content);
    }

    [When(@"I make POST requests with '(.*)' to '(.*)'")]
    public async Task WhenIMakePostRequestsWithTo(string fileList, string endpoint)
    {
        var files = fileList.Split(',');
        foreach (var file in files)
        {
            var json = JsonFilesRepo.Files[file];
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await Client.PostAsync(endpoint, content);
            Responses.Add(response);
        }
    }

    [Then(@"the response status code is '(.*)'")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        var expected = (HttpStatusCode)statusCode;
        Assert.Equal(expected, Response.StatusCode);
    }

    [Then(@"the response status codes are '(.*)'")]
    public void ThenTheResponseStatusCodesAre(int statusCode)
    {
        Assert.True(Responses.Count > 0);
        var expected = (HttpStatusCode)statusCode;
        foreach (var response in Responses)
        {
            Assert.Equal(expected, response.StatusCode);
        }
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
        var expected = JsonSerializer.Deserialize<Order>(json, JsonSerializerOptions);
        Order = await Response.Content.ReadFromJsonAsync<Order>();
        Assert.Equal(expected, Order, new OrderComparer()!);
    }

    [Then(@"the response entities should be '(.*)'")]
    public async Task ThenTheResponseEntitiesShouldBe(string fileList)
    {
        Assert.True(Responses.Count > 0);
        var files = fileList.Split(',');
        for (var i = 0; i < Responses.Count; i++)
        {
            var json = JsonFilesRepo.Files[files[i]];
            var expected = JsonSerializer.Deserialize<Order>(json, JsonSerializerOptions);
            var order = await Responses[i].Content.ReadFromJsonAsync<Order>();
            Orders.Add(order);
            Assert.Equal(expected, order, new OrderComparer()!);
        }
    }

    [Then(@"the customer credit should equal (.*)")]
    public async Task ThenTheCustomerCreditShouldEqual(decimal creditAvailable)
    {
        if (Customer == null) return;
        var timeout = Responses.Count == 0
            ? ServiceSpecsSettings.SagaCompletionTimeout
            : ServiceSpecsSettings.MultipleSagaCompletionTimeout;
        await Task.Delay(timeout);
        var customer = await CustomerRepository.GetAsync(Customer.Id);
        Assert.Equal(creditAvailable, customer?.CreditAvailable);
    }

    [Then(@"the inventory quantity should equal (.*)")]
    public async Task ThenTheInventoryLevelShouldBe(int amountAvailable)
    {
        if (Inventory == null) return;
        var timeout = Responses.Count == 0
            ? ServiceSpecsSettings.SagaCompletionTimeout
            : ServiceSpecsSettings.MultipleSagaCompletionTimeout;
        await Task.Delay(timeout);
        var inventory = await InventoryRepository.GetAsync(ServiceSpecsSettings.InventoryId);
        Assert.Equal(amountAvailable, inventory?.AmountAvailable);
    }

    [Then(@"the order state should be '(.*)'")]
    public async Task ThenTheOrderStateShouldBe(OrderState state)
    {
        if (Order == null) return;
        var timeout = Responses.Count == 0
            ? ServiceSpecsSettings.SagaCompletionTimeout
            : ServiceSpecsSettings.MultipleSagaCompletionTimeout;
        await Task.Delay(timeout);
        var orderState = await OrderRepository.GetOrderStateAsync(Order.Id);
        Assert.Equal(state, orderState);
    }

    [Then(@"the order states should be '(.*)'")]
    public async Task ThenTheOrderStatesShouldBe(OrderState state)
    {
        Assert.True(Orders.Count > 0);
        var timeout = Responses.Count == 0
            ? ServiceSpecsSettings.SagaCompletionTimeout
            : ServiceSpecsSettings.MultipleSagaCompletionTimeout;
        await Task.Delay(timeout);
        foreach (var order in Orders)
        {
            var orderState = await OrderRepository.GetOrderStateAsync(order!.Id);
            Assert.Equal(state, orderState);
        }
    }
}