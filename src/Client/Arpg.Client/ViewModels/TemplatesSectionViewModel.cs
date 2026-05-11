using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;

namespace Arpg.Client.ViewModels;

public class TemplatesSectionViewModel : SectionViewModelBase
{
    public TemplatesSectionViewModel(INavigationServiceFactory navFactory) : base(navFactory)
    {
        LocalNavigation.NavigateTo<TemplatesListViewModel>();
    }
}
