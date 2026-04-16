using System.Threading.Channels;

namespace Arpg.Infrastructure.Services.Background;

public record EmailMessage(string Email, string Code);

public class EmailBackgroundQueue
{
    private readonly Channel<EmailMessage> _queue;

    public EmailBackgroundQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        };
        _queue = Channel.CreateUnbounded<EmailMessage>(options);
    }

    public async ValueTask EnqueueAsync(EmailMessage message)
    {
        await _queue.Writer.WriteAsync(message);
    }

    public IAsyncEnumerable<EmailMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAllAsync(cancellationToken);
    }
}
