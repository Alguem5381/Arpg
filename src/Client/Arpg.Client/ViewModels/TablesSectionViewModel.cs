using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;

namespace Arpg.Client.ViewModels;

public class TablesSectionViewModel : SectionViewModelBase
{
    public TablesSectionViewModel(INavigationServiceFactory navFactory) : base(navFactory)
    {
        LocalNavigation.NavigateTo<TablesListViewModel>();
    }
}
