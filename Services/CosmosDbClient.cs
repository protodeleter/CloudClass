
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace AvaloniaApplication1.Services;

public sealed class CosmosDbClient : IDisposable
{
    private readonly CosmosClient _client;

    private const string Endpoint = "https://localhost:8081/";

    private const string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    public CosmosDbClient()
    {
        _client = new CosmosClient(
            Endpoint,
            Key,
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            });
    }

    // Create database and container if they don't exist
    public async Task CreateDbAsync(
        string databaseName,
        string containerName,
        string partitionKeyPath = "/id")
    {



        DatabaseResponse response =
            await _client.CreateDatabaseIfNotExistsAsync(
                databaseName);

        Debug.Write(response);

        Database database = response.Database;

        await database.CreateContainerIfNotExistsAsync(
            id: containerName,
            partitionKeyPath: partitionKeyPath);
    }

    // Verify an existing container
    public async Task ConnectAsync(
        string databaseName,
        string containerName)
    {
        Container container = _client.GetContainer(
            databaseName,
            containerName);

        await container.ReadContainerAsync();
    }

    // Insert a document
    public async Task InsertAsync<T>(
        string databaseName,
        string containerName,
        T document,
        string partitionKey)
    {
        Container container = _client.GetContainer(
            databaseName,
            containerName);

        await container.CreateItemAsync(
            document,
            new PartitionKey(partitionKey));
    }

    // Read a document
    public async Task<T> GetAsync<T>(
        string databaseName,
        string containerName,
        string id,
        string partitionKey)
    {
        Container container = _client.GetContainer(
            databaseName,
            containerName);

        ItemResponse<T> response =
            await container.ReadItemAsync<T>(
                id,
                new PartitionKey(partitionKey));

        return response.Resource;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
