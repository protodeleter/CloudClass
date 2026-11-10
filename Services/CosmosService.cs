using Microsoft.Azure.Cosmos;
using _pvlnsnv.Models;
public class CosmosService
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    public CosmosService()
    {
        string endpoint = "https://localhost:8081";
        string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        _client = new CosmosClient(endpoint, key);

        var database = _client.CreateDatabaseIfNotExistsAsync("MyDatabase").Result;

        _container = database.Database
            .CreateContainerIfNotExistsAsync("Customers", "/id")
            .Result
            .Container;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _container.CreateItemAsync(
            customer,
            new PartitionKey(customer.id));
    }
}