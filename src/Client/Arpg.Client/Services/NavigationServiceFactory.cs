using System;
using Arpg.Client.Abstractions;

namespace Arpg.Client.Services;

public class NavigationServiceFactory(IServiceProvider serviceProvider) : INavigationServiceFactory
{
    public INavigationServices CreateLocalNavigation()
    {
        return new NavigationService(serviceProvider);
    }
}
