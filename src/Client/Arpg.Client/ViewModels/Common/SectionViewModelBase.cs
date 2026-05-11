using Arpg.Client.Abstractions;

namespace Arpg.Client.ViewModels.Common;

public abstract class SectionViewModelBase : ViewModelBase
{
    public INavigationServices LocalNavigation { get; }

    protected SectionViewModelBase(INavigationServiceFactory navigationFactory)
    {
        LocalNavigation = navigationFactory.CreateLocalNavigation();
    }
}
