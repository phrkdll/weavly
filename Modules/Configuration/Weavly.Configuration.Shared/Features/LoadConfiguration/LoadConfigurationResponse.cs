namespace Weavly.Configuration.Shared.Features.LoadConfiguration;

public sealed record LoadConfigurationResponse(string Module, IEnumerable<ConfigurationItem> Items)
{
    private ConfigurationItem? Get(string name, string category)
    {
        return Items.SingleOrDefault(i => i.Name == name && i.Category == category);
    }

    public string? GetString(string name, string category)
    {
        return Get(name, category)?.AsString();
    }

    public bool? GetBool(string name, string category)
    {
        return Get(name, category)?.AsBool();
    }

    public int? GetInt(string name, string category)
    {
        return Get(name, category)?.AsInt();
    }

    public double? GetDouble(string name, string category)
    {
        return Get(name, category)?.AsDouble();
    }
}