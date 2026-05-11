using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Contracts.Dto.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public partial class SheetsListViewModel : ViewModelBase
{
    private readonly ISheetServices _sheetServices;
    private readonly ITemplateServices _templateServices;
    [ObservableProperty] private ObservableCollection<SimpleSheetDto> _sheets = [];
    [ObservableProperty] private bool _isCreateMenuOpen;
    [ObservableProperty] private string _newSheetName = string.Empty;
    [ObservableProperty] private SimpleTemplateDto? _selectedTemplate;
    [ObservableProperty] private ObservableCollection<SimpleTemplateDto> _availableTemplates = [];

    public SheetsListViewModel(ISheetServices sheetServices, ITemplateServices templateServices)
    {
        _sheetServices = sheetServices;
        _templateServices = templateServices;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var result = await _sheetServices.GetListAsync();
        if (result.IsSuccess)
            Sheets = new ObservableCollection<SimpleSheetDto>(result.Value);
    }

    [RelayCommand]
    private async Task OpenCreateMenu()
    {
        var templatesResult = await _templateServices.GetListAsync();
        if (templatesResult.IsSuccess) AvailableTemplates = new ObservableCollection<SimpleTemplateDto>(templatesResult.Value);
        IsCreateMenuOpen = true;
    }

    [RelayCommand]
    private void CloseCreateMenu()
    {
        IsCreateMenuOpen = false;
        NewSheetName = string.Empty;
        SelectedTemplate = null;
    }

    [RelayCommand]
    private async Task ConfirmCreate()
    {
        if (string.IsNullOrWhiteSpace(NewSheetName) || SelectedTemplate == null) return;
        var result = await _sheetServices.CreateAsync(new NewSheetDto(NewSheetName, SelectedTemplate.Id));
        if (result.IsSuccess)
        {
            CloseCreateMenu();
            await LoadDataAsync();
        }
    }

    [ObservableProperty] private bool _isDeleteMenuOpen;
    private Guid _sheetToDeleteId;

    [RelayCommand]
    private void OpenDeleteMenu(Guid id)
    {
        _sheetToDeleteId = id;
        IsDeleteMenuOpen = true;
    }

    [RelayCommand]
    private void CloseDeleteMenu()
    {
        IsDeleteMenuOpen = false;
        _sheetToDeleteId = Guid.Empty;
    }

    [RelayCommand]
    private async Task ConfirmDelete()
    {
        if (_sheetToDeleteId == Guid.Empty) return;
        var result = await _sheetServices.DeleteAsync(_sheetToDeleteId);
        if (result.IsSuccess)
        {
            CloseDeleteMenu();
            await LoadDataAsync();
        }
    }
}
