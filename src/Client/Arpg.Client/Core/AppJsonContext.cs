using Arpg.Contracts.Dto.General;
using Arpg.Contracts.Dto.User;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.Template;
using Arpg.Contracts.Dto.Sheet;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Arpg.Client.Core;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(LoginDto))]
[JsonSerializable(typeof(NewUserDto))]
[JsonSerializable(typeof(SuccessLoginDto))]
[JsonSerializable(typeof(CodeDto))]
[JsonSerializable(typeof(ValidateCodeDto))]
[JsonSerializable(typeof(ErrorResponseDto))]
[JsonSerializable(typeof(ApiError))]
[JsonSerializable(typeof(List<SimpleGameTableDto>))]
[JsonSerializable(typeof(List<SimpleTemplateDto>))]
[JsonSerializable(typeof(List<SimpleSheetDto>))]
[JsonSerializable(typeof(NewTableDto))]
[JsonSerializable(typeof(NewTemplateDto))]
[JsonSerializable(typeof(NewSheetDto))]
[JsonSerializable(typeof(SuccessCreateDto))]
[JsonSerializable(typeof(DeleteTemplateDto))]
internal partial class AppJsonContext : JsonSerializerContext;