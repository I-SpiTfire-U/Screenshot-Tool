using System.Text.Json.Serialization;

namespace Screenshot_Tool.src;

public sealed class MonitorInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; init; }

    [JsonPropertyName("height")]
    public int Height { get; init; }

    [JsonPropertyName("x")]
    public int X { get; init; }

    [JsonPropertyName("y")]
    public int Y { get; init; }

    public string GetGeometryAsString()
        => $"{X},{Y} {Width}x{Height}";
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(MonitorInfo[]))]
internal partial class MonitorJsonContext : JsonSerializerContext;