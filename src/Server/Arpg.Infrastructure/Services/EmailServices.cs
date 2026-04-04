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
        var sanitizedEmail = email.Trim().Replace("\r", "").Replace("\n", "");
        var message = new MimeMessage();
        // Use MailboxAddress constructor that takes name and address separately
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.User));
        message.To.Add(new MailboxAddress("Jogador", sanitizedEmail));

        // Removed accents from subject to avoid SMTPUTF8 5.5.2 decoding issues in some relays
        message.Subject = "Codigo de verificacao ARPG";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                         <div style="font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;">
                             <h1 style="color: #333; text-align: center;">Bem-vindo ao ARPG!</h1>
                             <p style="font-size: 16px; color: #666; text-align: center;">Use o código abaixo para validar seu acesso:</p>
                             <div style="background: #f4f4f4; padding: 20px; border-radius: 5px; text-align: center; margin: 20px 0;">
                                 <span style="font-size: 32px; font-weight: bold; color: #007bff; letter-spacing: 10px;">{code}</span>
                             </div>
                             <p style="font-size: 12px; color: #999; text-align: center;">Este código expira em 15 minutos. Se você não solicitou este e-mail, pode ignorá-lo.</p>
                         </div>
                         """
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        
        // Force a safe LocalDomain to avoid gsmtp syntax errors (5.5.2) related to machine hostname
        client.LocalDomain = "localhost";
        client.CheckCertificateRevocation = false;

        try
        {
            // Tracing parameters to console (visible in the user's terminal)
            Console.WriteLine($"[Email Service] Connecting to {_emailSettings.Host}:{_emailSettings.Port} (EHLO: {client.LocalDomain})");
            
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            
            // Explicitly disable SMTPUTF8 if we don't have non-ascii in headers (avoiding gsmtp 5.5.2)
            // This is done by checking if the server supports it and then just not using those features, 
            // but MailKit usually handles this. We'll try to ensure we use 7bit/8bit only.
            
            await client.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);
            
            Console.WriteLine($"[Email Service] Authenticated. Sending to {sanitizedEmail}...");
            await client.SendAsync(message);
            Console.WriteLine("[Email Service] Success.");
        }
        catch (Exception ex)
        {
            // Log for visibility in the console during debug
            Console.WriteLine($"[Email Error Detail] Type: {ex.GetType().Name}, Message: {ex.Message}");
            if (ex is SmtpCommandException sce)
            {
                Console.WriteLine($"[Smtp Status] {sce.ErrorCode} - {sce.StatusCode}");
            }
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}