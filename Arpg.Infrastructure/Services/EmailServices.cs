using Arpg.Application.Abstractions;
using Arpg.Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Arpg.Infrastructure.Services;

public class EmailServices(IOptions<EmailSettings> options) : IEmailServices
{
    private readonly EmailSettings _emailSettings = options.Value;
    
    public async Task SendCodeVerificationEmailAsync(string email, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.User));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Seu código de verificação Arpg";

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = $"""
                                    <h1>Bem-vindo ao Arpg!</h1>
                                    <p>Use o código abaixo para validar seu acesso:</p>
                                    <h2 style='color: #007bff; letter-spacing: 5px;'>{code}</h2>
                                    <p>Este código expira em 15 minutos.</p>
                    """
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}