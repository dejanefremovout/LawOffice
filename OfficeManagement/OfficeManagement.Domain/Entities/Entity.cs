using Newtonsoft.Json;

namespace OfficeManagement.Domain.Entities;

/// <summary>
/// Base entity for persisted domain objects.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Initializes a new entity with a generated identifier.
    /// </summary>
    protected Entity()
    {
        _Id = Guid.NewGuid().ToString();
    }

    private string _Id;

    /// <summary>
    /// Gets the persistence identifier used by the backing store.
    /// </summary>
    [JsonProperty("id")]
    public virtual string Id
    {
        get => _Id; protected set => _Id = value;
    }
}