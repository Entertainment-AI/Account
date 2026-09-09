using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Account.Application.Common.Converters;

public class FlexibleDateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string StandardDateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateStr = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateStr))
        {
            return default;
        }

        if (DateOnly.TryParseExact(dateStr, StandardDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            return dateOnly;
        }

        if (DateOnly.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOnly))
        {
            return dateOnly;
        }

        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new JsonException($"Unable to parse \"{dateStr}\" as DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(StandardDateFormat, CultureInfo.InvariantCulture));
    }
}

public class FlexibleNullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    private const string StandardDateFormat = "yyyy-MM-dd";

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var dateStr = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateStr))
        {
            return null;
        }

        if (DateOnly.TryParseExact(dateStr, StandardDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            return dateOnly;
        }

        if (DateOnly.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOnly))
        {
            return dateOnly;
        }

        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new JsonException($"Unable to parse \"{dateStr}\" as DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString(StandardDateFormat, CultureInfo.InvariantCulture));
        }
    }
}
