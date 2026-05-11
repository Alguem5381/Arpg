using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public partial class TemplatesListViewModel : ViewModelBase
{
    private readonly ITemplateServices _templateServices;
    [ObservableProperty] private ObservableCollection<SimpleTemplateDto> _templates = [];
    [ObservableProperty] private bool _isCreateMenuOpen;
    [ObservableProperty] private string _newTemplateName = string.Empty;
    public Action<Guid>? OnOpenTemplate { get; set; }

    public TemplatesListViewModel(ITemplateServices templateServices)
    {
        _templateServices = templateServices;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var result = await _templateServices.GetListAsync();
        if (result.IsSuccess)
            Templates = new ObservableCollection<SimpleTemplateDto>(result.Value);
    }

    [RelayCommand]
    private void OpenCreateMenu() => IsCreateMenuOpen = true;

    [RelayCommand]
    private void OpenTemplate(Guid id) => OnOpenTemplate?.Invoke(id);

    [RelayCommand]
    private void CloseCreateMenu()
    {
        IsCreateMenuOpen = false;
        NewTemplateName = string.Empty;
    }

    [RelayCommand]
    private async Task ConfirmCreate()
    {
        if (string.IsNullOrWhiteSpace(NewTemplateName)) return;
        var result = await _templateServices.CreateAsync(new NewTemplateDto(NewTemplateName));
        if (result.IsSuccess)
        {
            CloseCreateMenu();
            await LoadDataAsync();
        }
    }

    [ObservableProperty] private bool _isDeleteMenuOpen;
    [ObservableProperty] private string _deletePassword = string.Empty;
    private Guid _templateToDeleteId;

    [RelayCommand]
    private void OpenDeleteMenu(Guid id)
    {
        _templateToDeleteId = id;
        IsDeleteMenuOpen = true;
    }

    [RelayCommand]
    private void CloseDeleteMenu()
    {
        IsDeleteMenuOpen = false;
        DeletePassword = string.Empty;
        _templateToDeleteId = Guid.Empty;
    }

    [RelayCommand]
    private async Task ConfirmDelete()
    {
        if (_templateToDeleteId == Guid.Empty || string.IsNullOrWhiteSpace(DeletePassword)) return;
        var result = await _templateServices.DeleteAsync(new DeleteTemplateDto(_templateToDeleteId, DeletePassword));
        if (result.IsSuccess)
        {
            CloseDeleteMenu();
            await LoadDataAsync();
        }
    }
}
