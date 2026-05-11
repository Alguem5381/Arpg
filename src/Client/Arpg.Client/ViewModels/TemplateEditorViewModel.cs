using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Arpg.Client.Abstractions;
using Arpg.Client.ViewModels.Common;
using Arpg.Contracts.Dto.Structure;
using Arpg.Contracts.Dto.Template;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Enums.Template;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arpg.Client.ViewModels;

public partial class TemplateEditorViewModel : ViewModelBase
{
    private readonly ITemplateServices _templateServices;
    private Guid _templateId;

    // IDs de categorias e campos originais que foram deletados localmente
    private readonly ObservableCollection<Guid> _deletedCategoryIds = [];
    private readonly ObservableCollection<Guid> _deletedFieldIds = [];

    // IDs carregados do servidor (para diferenciar Add vs Edit no batch)
    private readonly System.Collections.Generic.HashSet<Guid> _serverCategoryIds = [];
    private readonly System.Collections.Generic.HashSet<Guid> _serverFieldIds = [];

    [ObservableProperty] private string _templateName = string.Empty;
    [ObservableProperty] private string _templateDescription = string.Empty;

    public ObservableCollection<TemplateCategoryViewModel> Categories { get; } = [];

    public Action? OnGoBack { get; set; }

    public TemplateEditorViewModel(ITemplateServices templateServices)
    {
        _templateServices = templateServices;
    }

    public void Initialize(Guid templateId)
    {
        _templateId = templateId;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var result = await _templateServices.GetAsync(_templateId);
        if (result.IsSuccess && result.Value.Structure != null)
        {
            TemplateName = result.Value.Name;
            TemplateDescription = result.Value.Description ?? string.Empty;
            Categories.Clear();
            _serverCategoryIds.Clear();
            _serverFieldIds.Clear();
            _deletedCategoryIds.Clear();
            _deletedFieldIds.Clear();

            foreach (var cat in result.Value.Structure.Categories.OrderBy(c => c.Order))
            {
                _serverCategoryIds.Add(cat.Id);
                var catVm = new TemplateCategoryViewModel(cat, OnDeleteCategory, OnDeleteField);
                var fieldsForCat = result.Value.Structure.Fields
                    .Where(f => f.CategoryId == cat.Id)
                    .OrderBy(f => f.Order);
                foreach (var f in fieldsForCat)
                {
                    _serverFieldIds.Add(f.Id);
                    catVm.Fields.Add(new TemplateFieldViewModel(f));
                }
                Categories.Add(catVm);
            }
        }
    }

    // Callbacks para deletar (chamados por cada TemplateCategoryViewModel filho)
    private void OnDeleteCategory(Guid categoryId)
    {
        var vm = Categories.FirstOrDefault(c => c.Id == categoryId);
        if (vm == null) return;

        // Campos da categoria também são deletados — só rastrear os do servidor
        foreach (var field in vm.Fields.Where(f => _serverFieldIds.Contains(f.Id)))
            _deletedFieldIds.Add(field.Id);

        Categories.Remove(vm);

        // Só rastrear deleção se era um item que já existia no servidor
        if (_serverCategoryIds.Contains(categoryId))
            _deletedCategoryIds.Add(categoryId);

        RecalculateOrders();
    }

    private void OnDeleteField(Guid fieldId, Guid categoryId)
    {
        var cat = Categories.FirstOrDefault(c => c.Id == categoryId);
        var field = cat?.Fields.FirstOrDefault(f => f.Id == fieldId);
        if (field == null) return;

        cat!.Fields.Remove(field);

        if (_serverFieldIds.Contains(fieldId))
            _deletedFieldIds.Add(fieldId);

        cat.RecalculateFieldOrders();
    }

    private void RecalculateOrders()
    {
        for (var i = 0; i < Categories.Count; i++)
            Categories[i].Order = i;
    }

    [RelayCommand]
    private void GoBack() => OnGoBack?.Invoke();

    [RelayCommand]
    private void AddCategory()
    {
        var newOrder = Categories.Count;
        Categories.Add(new TemplateCategoryViewModel(
            id: Guid.NewGuid(),
            name: $"Nova Categoria {newOrder + 1}",
            order: newOrder,
            onDelete: OnDeleteCategory,
            onDeleteField: OnDeleteField));
    }

    [RelayCommand]
    private void MoveCategoryUp(TemplateCategoryViewModel category)
    {
        var idx = Categories.IndexOf(category);
        if (idx <= 0) return;
        Categories.Move(idx, idx - 1);
        RecalculateOrders();
    }

    [RelayCommand]
    private void MoveCategoryDown(TemplateCategoryViewModel category)
    {
        var idx = Categories.IndexOf(category);
        if (idx < 0 || idx >= Categories.Count - 1) return;
        Categories.Move(idx, idx + 1);
        RecalculateOrders();
    }

    [RelayCommand]
    private void MoveFieldUp(TemplateFieldViewModel field)
    {
        var cat = Categories.FirstOrDefault(c => c.Id == field.CategoryId);
        if (cat == null) return;
        var idx = cat.Fields.IndexOf(field);
        if (idx <= 0) return;
        cat.Fields.Move(idx, idx - 1);
        cat.RecalculateFieldOrders();
    }

    [RelayCommand]
    private void MoveFieldDown(TemplateFieldViewModel field)
    {
        var cat = Categories.FirstOrDefault(c => c.Id == field.CategoryId);
        if (cat == null) return;
        var idx = cat.Fields.IndexOf(field);
        if (idx < 0 || idx >= cat.Fields.Count - 1) return;
        cat.Fields.Move(idx, idx + 1);
        cat.RecalculateFieldOrders();
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        var categories = new System.Collections.Generic.List<CategoryOpDto>();
        var fields = new System.Collections.Generic.List<FieldOpDto>();

        // Categorias deletadas
        foreach (var id in _deletedCategoryIds)
            categories.Add(new CategoryOpDto { Op = Operation.Delete, Id = id });

        // Campos deletados
        foreach (var id in _deletedFieldIds)
            fields.Add(new FieldOpDto { Op = Operation.Delete, Id = id, CategoryId = Guid.Empty });

        // Categorias existentes (Add ou Edit baseado na origem)
        for (var i = 0; i < Categories.Count; i++)
        {
            var cat = Categories[i];
            var op = _serverCategoryIds.Contains(cat.Id) ? Operation.Edit : Operation.Add;
            categories.Add(new CategoryOpDto
            {
                Op = op,
                Id = cat.Id,
                Name = cat.Name,
                Order = i
            });

            // Campos dessa categoria
            for (var j = 0; j < cat.Fields.Count; j++)
            {
                var f = cat.Fields[j];
                var fieldOp = _serverFieldIds.Contains(f.Id) ? Operation.Edit : Operation.Add;
                fields.Add(new FieldOpDto
                {
                    Op = fieldOp,
                    Id = f.Id,
                    CategoryId = cat.Id,
                    Name = f.Name,
                    IsRequired = f.IsRequired,
                    Order = j
                });
            }
        }

        var batch = new BatchStructureDto
        {
            TemplateId = _templateId,
            Categories = categories,
            Fields = fields
        };

        var result = await _templateServices.StructureUpdateAsync(batch);
        if (result.IsSuccess)
            await LoadDataAsync(); // Recarrega do servidor para sincronizar ordens
    }
}

public partial class TemplateCategoryViewModel : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private int _order;

    public Guid Id { get; }
    public ObservableCollection<TemplateFieldViewModel> Fields { get; } = [];

    private readonly Action<Guid>? _onDelete;
    private readonly Action<Guid, Guid>? _onDeleteField;

    public TemplateCategoryViewModel(TemplateCategory category, Action<Guid>? onDelete, Action<Guid, Guid>? onDeleteField)
    {
        Id = category.Id;
        Name = category.Name;
        Order = category.Order;
        _onDelete = onDelete;
        _onDeleteField = onDeleteField;
    }

    public TemplateCategoryViewModel(Guid id, string name, int order, Action<Guid>? onDelete, Action<Guid, Guid>? onDeleteField)
    {
        Id = id;
        Name = name;
        Order = order;
        _onDelete = onDelete;
        _onDeleteField = onDeleteField;
    }

    [RelayCommand]
    private void Delete() => _onDelete?.Invoke(Id);

    [RelayCommand]
    private void AddField()
    {
        var nextOrder = Fields.Count;
        Fields.Add(new TemplateFieldViewModel(
            id: Guid.NewGuid(),
            categoryId: Id,
            name: $"Novo Campo {nextOrder + 1}",
            type: FieldType.Text,
            isRequired: false,
            order: nextOrder,
            onDelete: _onDeleteField));
    }

    public void RecalculateFieldOrders()
    {
        for (var i = 0; i < Fields.Count; i++)
            Fields[i].Order = i;
    }
}

public partial class TemplateFieldViewModel : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private object? _defaultValue;
    [ObservableProperty] private string _type;
    [ObservableProperty] private bool _isRequired;
    [ObservableProperty] private int _order;

    public Guid Id { get; }
    public Guid CategoryId { get; }

    private readonly Action<Guid, Guid>? _onDelete;

    public TemplateFieldViewModel(TemplateField field, Action<Guid, Guid>? onDelete = null)
    {
        Id = field.Id;
        CategoryId = field.CategoryId;
        Name = field.Name;
        Type = field.Type.ToString();
        DefaultValue = field.DefaultValue;
        IsRequired = field.IsRequired;
        Order = field.Order;
        _onDelete = onDelete;
    }

    public TemplateFieldViewModel(Guid id, Guid categoryId, string name, FieldType type, bool isRequired, int order, Action<Guid, Guid>? onDelete = null)
    {
        Id = id;
        CategoryId = categoryId;
        Name = name;
        Type = type.ToString();
        IsRequired = isRequired;
        Order = order;
        _onDelete = onDelete;
    }

    [RelayCommand]
    private void Delete() => _onDelete?.Invoke(Id, CategoryId);
}
