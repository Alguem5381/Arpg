using Arpg.Client.ViewModels.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Arpg.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly TablesSectionViewModel _tablesSection;
    private readonly TemplatesSectionViewModel _templatesSection;
    private readonly SheetsSectionViewModel _sheetsSection;

    [ObservableProperty] private string _currentPageTitle = "Mesas";
    [ObservableProperty] private SectionViewModelBase? _currentSection;

    public MainViewModel(
        TablesSectionViewModel tablesSection,
        TemplatesSectionViewModel templatesSection,
        SheetsSectionViewModel sheetsSection)
    {
        _tablesSection = tablesSection;
        _templatesSection = templatesSection;
        _sheetsSection = sheetsSection;

        CurrentSection = _tablesSection;
    }

    [RelayCommand]
    private void ShowTables()
    {
        CurrentPageTitle = "Mesas";
        CurrentSection = _tablesSection;
    }

    [RelayCommand]
    private void ShowTemplates()
    {
        CurrentPageTitle = "Modelos";
        CurrentSection = _templatesSection;
    }

    [RelayCommand]
    private void ShowSheets()
    {
        CurrentPageTitle = "Fichas";
        CurrentSection = _sheetsSection;
    }
}
