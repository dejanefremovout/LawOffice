using Microsoft.Azure.Cosmos;
using System.Net;

var connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("COSMOS_CONNECTION_STRING is required.");
    return 1;
}

var disableSslValidation = string.Equals(
    Environment.GetEnvironmentVariable("COSMOS_DISABLE_SSL_VALIDATION"),
    "true",
    StringComparison.OrdinalIgnoreCase);

var options = new CosmosClientOptions
{
    ConnectionMode = ConnectionMode.Gateway,
    LimitToEndpoint = true,
    RequestTimeout = TimeSpan.FromSeconds(60)
};

if (disableSslValidation)
{
    options.HttpClientFactory = () =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return new HttpClient(handler, disposeHandler: true);
    };
}

var databases = new[] { "casemanagement", "officemanagement", "partymanagement" };

var containers = new (string Database, string Container, string PartitionKey)[]
{
    ("casemanagement", "cases", "/officeId"),
    ("casemanagement", "documentfiles", "/officeId"),
    ("casemanagement", "hearings", "/officeId"),
    ("officemanagement", "lawyers", "/officeId"),
    ("officemanagement", "offices", "/id"),
    ("partymanagement", "clients", "/officeId"),
    ("partymanagement", "opposingparties", "/officeId"),
};

using var client = new CosmosClient(connectionString, options);

// Phase 1: Create all databases first and let the emulator stabilize.
foreach (var db in databases)
{
    await RetryAsync(async () =>
    {
        var response = await client.CreateDatabaseIfNotExistsAsync(db);
        var created = response.StatusCode == HttpStatusCode.Created;
        Console.WriteLine(
            created
                ? $"Created database '{db}'"
                : $"Database '{db}' already exists");

        // Only wait for emulator stabilization when a new database was created.
        if (created)
            await Task.Delay(TimeSpan.FromSeconds(10));
    });
}

// Phase 2: Create containers one at a time with generous pauses
// to let partition migrations complete (substatus 1007).
foreach (var (db, container, pk) in containers)
{
    await RetryAsync(async () =>
    {
        var database = client.GetDatabase(db);
        var response = await database.CreateContainerIfNotExistsAsync(new ContainerProperties(container, pk));
        var created = response.StatusCode == HttpStatusCode.Created;
        Console.WriteLine(
            created
                ? $"Created container '{db}/{container}'"
                : $"Container '{db}/{container}' already exists");

        // Partition migration delays are needed only for newly created containers.
        if (created)
            await Task.Delay(TimeSpan.FromSeconds(15));
    });
}

Console.WriteLine("Cosmos DB emulator seeded successfully.");
return 0;

static async Task RetryAsync(Func<Task> action, int maxAttempts = 30)
{
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await action();
            return;
        }
        catch (Exception ex)
        {
            if (attempt >= maxAttempts)
                throw;
            var delay = Math.Min(10 * attempt, 60);
            Console.WriteLine($"  Attempt {attempt}/{maxAttempts} failed: {ex.Message}");
            Console.WriteLine($"  Retrying in {delay}s...");
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
    }
}
