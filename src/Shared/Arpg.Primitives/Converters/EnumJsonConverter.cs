using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arpg.Primitives.Converters;

public class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String ||
            Enum.TryParse(reader.GetString(), true, out T result))
            return default;

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}