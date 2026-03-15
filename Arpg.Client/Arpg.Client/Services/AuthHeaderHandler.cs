using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;

namespace Arpg.Client.Services;

public class AuthHeaderHandler(IUserSession userSession) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (userSession.IsAuthenticated)
            request.Headers.Authorization = new("Bearer", userSession.Token);

        return await base.SendAsync(request, cancellationToken);
    }
}