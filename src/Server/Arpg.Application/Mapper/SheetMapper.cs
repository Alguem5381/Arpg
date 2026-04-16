using Arpg.Contracts.Dto.Sheet;
using Arpg.Core.Models;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class SheetMapper
{
    [MapperIgnoreTarget(nameof(Sheet.Id))]
    [MapperIgnoreTarget(nameof(Sheet.OwnerId))]
    public partial Sheet SheetCreateDto(CreateDto dto);

    [MapperIgnoreSource(nameof(Sheet.TemplateId))]
    public partial SheetDto SheetToSheetDto(Sheet sheet);
}