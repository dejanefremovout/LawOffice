using LawOffice.AI.Retrieval;
using Microsoft.Azure.Cosmos;

namespace LawOffice.AI.Tests.Retrieval;

/// <summary>
/// Proves the mandatory data-layer tenant filter without a live Cosmos: the search query always carries
/// <c>WHERE c.officeId = @officeId</c> and the bound parameter. This is the second, independent guard
/// that backs up the partition-key scoping (defence in depth).
/// </summary>
public class CosmosVectorStoreQueryTests
{
    [Fact]
    public void BuildSearchQuery_always_filters_by_the_tenant()
    {
        QueryDefinition query = CosmosVectorStore.BuildSearchQuery("office-a", 5, new float[] { 0.1f, 0.2f });

        query.QueryText.ShouldContain("c.officeId = @officeId");

        IReadOnlyList<(string Name, object Value)> parameters = query.GetQueryParameters();
        parameters.ShouldContain(p => p.Name == "@officeId" && (string)p.Value == "office-a");
        parameters.ShouldContain(p => p.Name == "@topK");
        parameters.ShouldContain(p => p.Name == "@embedding");
    }

    [Fact]
    public void BuildSearchQuery_orders_by_vector_distance()
    {
        QueryDefinition query = CosmosVectorStore.BuildSearchQuery("office-a", 3, new float[] { 1f });

        query.QueryText.ShouldContain("ORDER BY VectorDistance");
    }
}
