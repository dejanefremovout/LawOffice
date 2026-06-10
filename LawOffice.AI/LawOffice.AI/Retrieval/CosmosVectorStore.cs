using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace LawOffice.AI.Retrieval;

/// <summary>
/// <see cref="IVectorStore"/> backed by Azure Cosmos DB integrated vector search (DiskANN). Chosen for
/// LawOffice because the app already runs Cosmos serverless: embeddings add zero new infrastructure,
/// cost ~nothing at idle, and reuse the proven <c>/officeId</c> tenant partition.
///
/// <para>
/// Tenant isolation is enforced twice (defence in depth): every query both scopes the
/// <see cref="QueryRequestOptions.PartitionKey"/> to the tenant <i>and</i> carries a
/// <c>WHERE c.officeId = @officeId</c> filter. The filter construction is factored into the pure,
/// unit-testable <see cref="BuildSearchQuery"/> so the mandatory scoping can be proven without a live store.
/// </para>
/// </summary>
public sealed class CosmosVectorStore : IVectorStore
{
    private readonly CosmosClient _client;
    private readonly RetrievalSettings _settings;
    private readonly int _dimensions;
    private readonly ILogger<CosmosVectorStore> _logger;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private Container? _container;

    public CosmosVectorStore(
        CosmosClient client,
        RetrievalSettings settings,
        int embeddingDimensions,
        ILogger<CosmosVectorStore> logger)
    {
        _client = client;
        _settings = settings;
        _dimensions = embeddingDimensions;
        _logger = logger;
    }

    public async Task UpsertAsync(string officeId, IReadOnlyList<VectorRecord> records, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentNullException.ThrowIfNull(records);

        Container container = await GetContainerAsync(cancellationToken).ConfigureAwait(false);
        PartitionKey partitionKey = new(officeId);

        foreach (VectorRecord record in records)
        {
            if (record.OfficeId != officeId)
            {
                throw new ArgumentException(
                    $"Record '{record.Id}' has officeId '{record.OfficeId}' but is being upserted under '{officeId}'.",
                    nameof(records));
            }

            await container.UpsertItemAsync(record, partitionKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyList<ScoredChunk>> SearchAsync(
        string officeId,
        ReadOnlyMemory<float> queryEmbedding,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(topK);

        Container container = await GetContainerAsync(cancellationToken).ConfigureAwait(false);
        QueryDefinition query = BuildSearchQuery(officeId, topK, queryEmbedding);

        // Scope the iterator to the tenant partition as well — the partition key and the WHERE clause are
        // independent guards, so a mistake in either alone cannot cross the tenant boundary.
        QueryRequestOptions options = new() { PartitionKey = new PartitionKey(officeId) };

        List<ScoredChunk> results = [];
        using FeedIterator<CosmosHit> iterator = container.GetItemQueryIterator<CosmosHit>(query, requestOptions: options);
        while (iterator.HasMoreResults)
        {
            FeedResponse<CosmosHit> page = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            foreach (CosmosHit hit in page)
            {
                results.Add(new ScoredChunk(hit.Record, hit.Score));
            }
        }

        return results;
    }

    /// <summary>
    /// Builds the tenant-scoped nearest-neighbour query. Always emits <c>WHERE c.officeId = @officeId</c>
    /// and orders by <c>VectorDistance</c>; exposed for unit testing the mandatory tenant filter.
    /// </summary>
    public static QueryDefinition BuildSearchQuery(string officeId, int topK, ReadOnlyMemory<float> queryEmbedding)
    {
        const string sql =
            "SELECT TOP @topK c AS record, VectorDistance(c.embedding, @embedding) AS score " +
            "FROM c " +
            "WHERE c.officeId = @officeId " +
            "ORDER BY VectorDistance(c.embedding, @embedding)";

        return new QueryDefinition(sql)
            .WithParameter("@topK", topK)
            .WithParameter("@officeId", officeId)
            .WithParameter("@embedding", queryEmbedding.ToArray());
    }

    private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            return _container;
        }

        await _initLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return _container ??= await EnsureContainerAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Creates the container (if missing) with a cosine vector embedding policy and a DiskANN vector index
    // on /embedding. In production this provisioning would live in the deployment/seeder; here the store
    // ensures it so the demo is self-contained.
    private async Task<Container> EnsureContainerAsync(CancellationToken cancellationToken)
    {
        Embedding embedding = new()
        {
            Path = "/embedding",
            DataType = VectorDataType.Float32,
            DistanceFunction = DistanceFunction.Cosine,
            Dimensions = _dimensions,
        };

        ContainerProperties properties = new(_settings.ContainerId, "/officeId")
        {
            VectorEmbeddingPolicy = new VectorEmbeddingPolicy(new Collection<Embedding> { embedding }),
        };
        properties.IndexingPolicy.VectorIndexes.Add(new VectorIndexPath
        {
            Path = "/embedding",
            Type = VectorIndexType.DiskANN,
        });
        properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/embedding/*" });

        Database database = await _client
            .CreateDatabaseIfNotExistsAsync(_settings.DatabaseId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        ContainerResponse response = await database
            .CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Cosmos vector container '{Container}' ready (database '{Database}', {Dimensions} dims, cosine/DiskANN).",
            _settings.ContainerId, _settings.DatabaseId, _dimensions);

        return response.Container;
    }

    // Projection of "SELECT c AS record, VectorDistance(...) AS score".
    private sealed class CosmosHit
    {
        [JsonPropertyName("record")]
        public VectorRecord Record { get; init; } = default!;

        [JsonPropertyName("score")]
        public double Score { get; init; }
    }
}
