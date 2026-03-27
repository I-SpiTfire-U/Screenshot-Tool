using System.Text.Json.Serialization;

namespace Screenshot_Tool.src.Types;

public sealed class Window
{
  public String DisplayTitle { get; set; } = String.Empty;

  [JsonPropertyName("pid")]
  public Int32 ProcessID { get; init; } = 0;

  [JsonPropertyName("class")]
  public String Class { get; init; } = String.Empty;

  [JsonPropertyName("title")]
  public String Title { get; init; } = String.Empty;

  [JsonPropertyName("at")]
  public Int32[] Position { get; init; } = [];

  [JsonPropertyName("size")]
  public Int32[] Size { get; init; } = [];

  [JsonIgnore]
  public Int32 X =>
    Position is [Int32 x, _] ? x : 0;

  [JsonIgnore]
  public Int32 Y =>
    Position is [_, Int32 y] ? y : 0;

  [JsonIgnore]
  public Int32 Width =>
    Size is [Int32 w, _] ? w : 0;

  [JsonIgnore]
  public Int32 Height =>
    Size is [_, Int32 h] ? h : 0;

  public String GetGrimGeometry() =>
    $"{X},{Y} {Width}x{Height}";
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Window[]))]
internal partial class WindowJsonContext : JsonSerializerContext;
