using System.Text.Json.Serialization;

namespace Arpg.Shared.Enums.Template;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Operation
{
    Add,
    Edit,
    Delete
}