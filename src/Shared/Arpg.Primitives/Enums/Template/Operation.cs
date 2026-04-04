using System.Text.Json.Serialization;

namespace Arpg.Primitives.Enums.Template;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Operation
{
    Add,
    Edit,
    Delete
}