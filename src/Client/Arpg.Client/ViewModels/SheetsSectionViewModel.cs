using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;

namespace Arpg.Client.ViewModels;

public class SheetsSectionViewModel : SectionViewModelBase
{
    public SheetsSectionViewModel(INavigationServiceFactory navFactory) : base(navFactory)
    {
        LocalNavigation.NavigateTo<SheetsListViewModel>(vm =>
        {
            vm.OnOpenSheet = id => LocalNavigation.NavigateTo<SheetEditorViewModel>(editor =>
            {
                editor.GoBack = () => LocalNavigation.GoBack();
                _ = editor.LoadDataAsync(id);
            });
        });
    }
}
