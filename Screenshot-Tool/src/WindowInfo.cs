using System.Text.Json.Serialization;

namespace Screenshot_Tool.src;

public sealed class WindowInfo
{
    [JsonPropertyName("class")]
    public string Class { get; init; } = string.Empty;

    [JsonPropertyName("at")]
    public int[] At { get; init; } = [];

    [JsonPropertyName("size")]
    public int[] Size { get; init; } = [];

    [JsonIgnore]
    public int X => At.Length > 0 ? At[0] : 0;

    [JsonIgnore]
    public int Y => At.Length > 1 ? At[1] : 0;

    [JsonIgnore]
    public int Width => Size.Length > 0 ? Size[0] : 0;

    [JsonIgnore]
    public int Height => Size.Length > 1 ? Size[1] : 0;

    public string GetGeometryAsString()
        => $"{X},{Y} {Width}x{Height}";
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(WindowInfo[]))]
internal partial class WindowJsonContext : JsonSerializerContext;