namespace Arpg.Application.Abstractions;

public interface IEmailServices
{
    Task SendCodeVerificationEmailAsync(string email, string code);
}