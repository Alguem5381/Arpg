using Arpg.Application.Abstractions;
using Arpg.Infrastructure.Services.Background;

namespace Arpg.Infrastructure.Services;

public class EmailServices(EmailBackgroundQueue queue) : IEmailServices
{
    public async Task SendCodeVerificationEmailAsync(string email, string code)
    {
        await queue.EnqueueAsync(new EmailMessage(email, code));
    }
}