using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Auth;
using Arpg.Client.ViewModels;

namespace Arpg.Client.Services;

public class AuthHeaderHandler(IUserSession userSession, INavigationServices navigationServices) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (userSession.IsAuthenticated)
            request.Headers.Authorization = new("Bearer", userSession.Token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        userSession.Token = null;
        navigationServices.JumpTo<AuthViewModel>(viewModel =>
        {
            viewModel.AuthenticationCompleted += () => navigationServices.NavigateTo<MainViewModel>();
        });

        return response;
    }
}