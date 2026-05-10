using System.Text.Json.Serialization;
using Arpg.Primitives.Converters;

namespace Arpg.Primitives.Enums.Template;

[JsonConverter(typeof(EnumJsonConverter<Operation>))]
public enum Operation
{
    Unknown,
    Add,
    Edit,
    Delete
}