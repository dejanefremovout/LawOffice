using Newtonsoft.Json;

namespace LawOffice.Domain.Entities;

public abstract class Entity
{
    protected Entity()
    {
        _Id = Guid.NewGuid().ToString();
    }

    private string _Id;

    [JsonProperty("id")]
    public virtual string Id
    {
        get => _Id; protected set => _Id = value;
    }
}