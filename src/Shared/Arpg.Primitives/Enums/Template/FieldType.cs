using System.Text.Json.Serialization;

namespace Arpg.Primitives.Enums.Template;

[JsonConverter(typeof(JsonStringEnumConverter<FieldType>))]
public enum FieldType
{
    Number,
    Text,
    TextArea
}