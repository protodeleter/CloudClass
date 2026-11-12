
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace AvaloniaApplication1.Services;

public sealed class CosmosDbClient : IDisposable
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    private const string Endpoint = "https://localhost:8081/";

    private const string Key =
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+" +
        "4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    public CosmosDbClient(
        string databaseName,
        string containerName)
    {
        _client = new CosmosClient(
            Endpoint,
            Key,
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            });

        _container = _client.GetContainer(
            databaseName,
            containerName);
    }

    // Verify that the database exists and is accessible
    public async Task ConnectAsync()
    {
        await _container.ReadContainerAsync();
    }

    // Insert a document
    public async Task InsertAsync<T>(
        T document,
        string partitionKey)
    {
        await _container.CreateItemAsync(
            document,
            new PartitionKey(partitionKey));
    }

    // Read a document
    public async Task<T> GetAsync<T>(
        string id,
        string partitionKey)
    {
        ItemResponse<T> response =
            await _container.ReadItemAsync<T>(
                id,
                new PartitionKey(partitionKey));

        return response.Resource;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
