using System.Text.Json.Serialization;

namespace Screenshot_Tool.src.Types;

public sealed class Monitor
{
  [JsonPropertyName("name")]
  public String Name { get; init; } = String.Empty;

  [JsonPropertyName("width")]
  public Int32 Width { get; init; } = 0;

  [JsonPropertyName("height")]
  public Int32 Height { get; init; } = 0;

  [JsonPropertyName("x")]
  public Int32 X { get; init; } = 0;

  [JsonPropertyName("y")]
  public Int32 Y { get; init; } = 0;

  public String GetGrimGeometry() =>
    $"{X},{Y} {Width}x{Height}";
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Monitor[]))]
internal partial class MonitorJsonContext : JsonSerializerContext;
