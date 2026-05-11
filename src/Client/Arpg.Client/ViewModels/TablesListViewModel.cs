using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.GameTable;
using Arpg.Contracts.Dto.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public partial class TablesListViewModel : ViewModelBase
{
    private readonly ITableServices _tableServices;
    private readonly ITemplateServices _templateServices;
    [ObservableProperty] private ObservableCollection<SimpleGameTableDto> _tables = [];
    [ObservableProperty] private bool _isCreateMenuOpen;
    [ObservableProperty] private string _newTableName = string.Empty;
    [ObservableProperty] private SimpleTemplateDto? _selectedTemplate;
    [ObservableProperty] private ObservableCollection<SimpleTemplateDto> _availableTemplates = [];

    public TablesListViewModel(ITableServices tableServices, ITemplateServices templateServices)
    {
        _tableServices = tableServices;
        _templateServices = templateServices;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var result = await _tableServices.GetListAsync();
        if (result.IsSuccess)
            Tables = new ObservableCollection<SimpleGameTableDto>(result.Value);
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
        NewTableName = string.Empty;
        SelectedTemplate = null;
    }

    [RelayCommand]
    private async Task ConfirmCreate()
    {
        if (string.IsNullOrWhiteSpace(NewTableName) || SelectedTemplate == null) return;
        var result = await _tableServices.CreateAsync(new NewTableDto(NewTableName, SelectedTemplate.Id));
        if (result.IsSuccess)
        {
            CloseCreateMenu();
            await LoadDataAsync();
        }
    }

    [ObservableProperty] private bool _isDeleteMenuOpen;
    private Guid _tableToDeleteId;

    [RelayCommand]
    private void OpenDeleteMenu(Guid id)
    {
        _tableToDeleteId = id;
        IsDeleteMenuOpen = true;
    }

    [RelayCommand]
    private void CloseDeleteMenu()
    {
        IsDeleteMenuOpen = false;
        _tableToDeleteId = Guid.Empty;
    }

    [RelayCommand]
    private async Task ConfirmDelete()
    {
        if (_tableToDeleteId == Guid.Empty) return;
        var result = await _tableServices.DeleteAsync(_tableToDeleteId);
        if (result.IsSuccess)
        {
            CloseDeleteMenu();
            await LoadDataAsync();
        }
    }
}
