using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Primitives.Enums.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public abstract partial class SheetFieldViewModel : ViewModelBase
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public FieldType Type { get; init; }

    public abstract void LoadValue(object? rawValue);
    public abstract object? GetValue();
}

public partial class TextFieldViewModel : SheetFieldViewModel
{
    [ObservableProperty] private string _textValue = string.Empty;
    public override void LoadValue(object? rawValue) => TextValue = rawValue?.ToString() ?? "";
    public override object? GetValue() => string.IsNullOrWhiteSpace(TextValue) ? null : TextValue;
}

public partial class TextAreaFieldViewModel : SheetFieldViewModel
{
    [ObservableProperty] private string _textValue = string.Empty;
    public override void LoadValue(object? rawValue) => TextValue = rawValue?.ToString() ?? "";
    public override object? GetValue() => string.IsNullOrWhiteSpace(TextValue) ? null : TextValue;
}

public partial class NumberFieldViewModel : SheetFieldViewModel
{
    [ObservableProperty] private decimal _numberValue;
    public override void LoadValue(object? rawValue)
    {
        if (rawValue is JsonElement je && je.TryGetDecimal(out var d))
            NumberValue = d;
        else if (rawValue != null && decimal.TryParse(rawValue.ToString(), out var parsedNumber))
            NumberValue = parsedNumber;
    }
    public override object? GetValue() => NumberValue;
}

public partial class BooleanFieldViewModel : SheetFieldViewModel
{
    [ObservableProperty] private bool _boolValue;
    public override void LoadValue(object? rawValue)
    {
        if (rawValue is JsonElement je && (je.ValueKind == JsonValueKind.True || je.ValueKind == JsonValueKind.False))
            BoolValue = je.GetBoolean();
        else if (rawValue != null && bool.TryParse(rawValue.ToString(), out var parsedBool))
            BoolValue = parsedBool;
    }
    public override object? GetValue() => BoolValue;
}


public partial class SheetCategoryViewModel : ViewModelBase
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public ObservableCollection<SheetFieldViewModel> Fields { get; init; } = [];
}

public partial class SheetEditorViewModel : ViewModelBase
{
    private readonly ISheetServices _sheetServices;
    private readonly ITemplateServices _templateServices;
    
    [ObservableProperty] private string _sheetName = string.Empty;
    [ObservableProperty] private bool _isLoading = true;
    
    private Guid _sheetId;
    
    public Action? GoBack { get; set; }

    public ObservableCollection<SheetCategoryViewModel> Categories { get; } = [];

    public SheetEditorViewModel(ISheetServices sheetServices, ITemplateServices templateServices)
    {
        _sheetServices = sheetServices;
        _templateServices = templateServices;
    }

    public async Task LoadDataAsync(Guid sheetId)
    {
        _sheetId = sheetId;
        IsLoading = true;
        Categories.Clear();
        
        var sheetResult = await _sheetServices.GetAsync(_sheetId);
        if (sheetResult.IsFailed)
        {
            IsLoading = false;
            return;
        }

        var sheet = sheetResult.Value;
        SheetName = sheet.Name;
        
        var templateResult = await _templateServices.GetAsync(sheet.TemplateId);
        if (templateResult.IsFailed)
        {
            IsLoading = false;
            return;
        }
        
        var template = templateResult.Value;
        
        var sortedCategories = template.Structure.Categories.OrderBy(c => c.Order).ToList();
        
        foreach (var cat in sortedCategories)
        {
            var catVm = new SheetCategoryViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Order = cat.Order
            };
            
            var fields = template.Structure.Fields
                .Where(f => f.CategoryId == cat.Id)
                .OrderBy(f => f.Order)
                .ToList();
                
            foreach (var field in fields)
            {
                SheetFieldViewModel fieldVm = field.Type switch
                {
                    FieldType.Number => new NumberFieldViewModel { Id = field.Id, Name = field.Name, Type = field.Type },
                    FieldType.Boolean => new BooleanFieldViewModel { Id = field.Id, Name = field.Name, Type = field.Type },
                    FieldType.TextArea => new TextAreaFieldViewModel { Id = field.Id, Name = field.Name, Type = field.Type },
                    _ => new TextFieldViewModel { Id = field.Id, Name = field.Name, Type = field.Type }
                };
                
                sheet.Data.TryGetValue(field.Id, out var val);
                fieldVm.LoadValue(val);
                
                catVm.Fields.Add(fieldVm);
            }
            
            Categories.Add(catVm);
        }
        
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        var data = new Dictionary<Guid, object?>();
        foreach (var cat in Categories)
        {
            foreach (var field in cat.Fields)
            {
                data[field.Id] = field.GetValue();
            }
        }
        
        var editResult = await _sheetServices.EditAsync(new EditSheetDto(_sheetId, SheetName));
        if (editResult.IsFailed) return;
        
        var dto = new ComputeSheetDto(_sheetId, data);
        await _sheetServices.ComputeDataAsync(dto);
        
        GoBack?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        GoBack?.Invoke();
    }
}
