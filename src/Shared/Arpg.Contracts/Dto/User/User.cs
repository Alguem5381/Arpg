namespace Arpg.Contracts.Dto.User;

public record LoginDto(string Username, string Password);
public record NewDto(string Username, string Email, string Password, string ConfirmPassword) : LoginDto(Username, Password);
public record EditDto(string DisplayName);
public record DeleteDto(string Password);
public record SuccessLoginDto(string Message, string Token);
public record InformationDto(string DisplayName, string Username);
public record UserDto(string DisplayName, string Username);
public record CodeDto(Guid Key);
public record ValidateCodeDto(Guid Key, string Value);