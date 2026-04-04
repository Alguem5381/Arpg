using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.User;

using System.Text.Json.Serialization;

namespace Arpg.Client.Core;

[JsonSourceGenerationOptions(
    WriteIndented = false, 
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]

[JsonSerializable(typeof(LoginDto))]
[JsonSerializable(typeof(NewDto))]
[JsonSerializable(typeof(SuccessLoginDto))]
[JsonSerializable(typeof(CodeDto))]
[JsonSerializable(typeof(ValidateCodeDto))]
[JsonSerializable(typeof(ErrorResponseDto))] 
[JsonSerializable(typeof(ApiError))]
// [JsonSerializable()]
internal partial class AppJsonContext : JsonSerializerContext
{
}