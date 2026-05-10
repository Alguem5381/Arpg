using System.Collections.Generic;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Contracts.Dto.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ITableServices _tableServices;
    private readonly ITemplateServices _templateServices;
    private readonly ISheetServices _sheetServices;

    [ObservableProperty] private string _currentPageTitle = "Carregando...";

    [ObservableProperty] private IEnumerable<object>? _currentItems;

    public List<SimpleGameTableDto> TablesCache { get; private set; } = [];
    public List<SimpleTemplateDto> TemplatesCache { get; private set; } = [];
    public List<SimpleSheetDto> SheetsCache { get; private set; } = [];

    public MainViewModel(
        ITableServices tableServices,
        ITemplateServices templateServices,
        ISheetServices sheetServices)
    {
        _tableServices = tableServices;
        _templateServices = templateServices;
        _sheetServices = sheetServices;

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var tablesTask = _tableServices.GetListAsync();
        var templatesTask = _templateServices.GetListAsync();
        var sheetsTask = _sheetServices.GetListAsync();

        await Task.WhenAll(tablesTask, templatesTask, sheetsTask);

        var tablesResult = await tablesTask;
        var templatesResult = await templatesTask;
        var sheetsResult = await sheetsTask;

        if (tablesResult.IsSuccess) TablesCache = tablesResult.Value;
        if (templatesResult.IsSuccess) TemplatesCache = templatesResult.Value;
        if (sheetsResult.IsSuccess) SheetsCache = sheetsResult.Value;

        ShowTables();
    }

    [RelayCommand]
    private void ShowTables()
    {
        CurrentPageTitle = "Mesas";
        CurrentItems = TablesCache;
    }

    [RelayCommand]
    private void ShowTemplates()
    {
        CurrentPageTitle = "Modelos";
        CurrentItems = TemplatesCache;
    }

    [RelayCommand]
    private void ShowSheets()
    {
        CurrentPageTitle = "Fichas";
        CurrentItems = SheetsCache;
    }
}
