using Arpg.Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Arpg.Infrastructure.Services.Background;

public class EmailDispatcherService(
    EmailBackgroundQueue queue,
    IOptions<EmailSettings> options) : BackgroundService
{
    private readonly EmailSettings _emailSettings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Email Dispatcher] Daemon started successfully.");

        await foreach (var message in queue.DequeueAsync(stoppingToken))
        {
            try
            {
                await ProcessEmailAsync(message.Email, message.Code, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Dispatcher Error] Failed to send email to {message.Email}. Details: {ex.Message}");
            }
        }
    }

    private async Task ProcessEmailAsync(string email, string code, CancellationToken stoppingToken)
    {
        var sanitizedEmail = email.Trim().Replace("\r", "").Replace("\n", "");
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.User));
        message.To.Add(new MailboxAddress("Jogador", sanitizedEmail));
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

        client.LocalDomain = "localhost";
        client.CheckCertificateRevocation = false;

        try
        {
            Console.WriteLine($"[Email Dispatcher] Connecting to {_emailSettings.Host}:{_emailSettings.Port}...");

            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls, stoppingToken);
            await client.AuthenticateAsync(_emailSettings.User, _emailSettings.Password, stoppingToken);

            Console.WriteLine($"[Email Dispatcher] Background sending to {sanitizedEmail}...");
            await client.SendAsync(message, stoppingToken);
            Console.WriteLine("[Email Dispatcher] Success.");
        }
        finally
        {
            await client.DisconnectAsync(true, stoppingToken);
        }
    }
}
