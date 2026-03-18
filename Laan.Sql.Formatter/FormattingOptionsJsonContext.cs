using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laan.Sql.Formatter
{
#if NET6_0_OR_GREATER
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
#endif
}
