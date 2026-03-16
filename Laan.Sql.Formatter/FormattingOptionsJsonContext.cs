using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laan.Sql.Formatter
{
    /// <summary>
    /// JSON source generation context for FormattingOptions
    /// </summary>
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    )]
    [JsonSerializable(typeof(FormattingOptions))]
    internal partial class FormattingOptionsJsonContext : JsonSerializerContext
    {
    }
}
