namespace Arpg.Contracts.Dto.General;

public record SuccessDto(string Message);

public record ErrorDto(string Error, string Code, Dictionary<string, object> Metadata);
public record ErrorResponseDto(List<ErrorDto> Errors);