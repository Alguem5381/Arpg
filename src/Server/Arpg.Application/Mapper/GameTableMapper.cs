using Arpg.Contracts.Dto.GameTable;
using Arpg.Core.Models.Tabletop;
using Arpg.Core.Models.Tabletop.Batch;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class GameTableMapper
{
    [MapperIgnoreSource(nameof(GameTable.PlayerIds))]
    [MapperIgnoreSource(nameof(GameTable.SheetIds))]
    public partial SimpleGameTableDto ToGameTableInformationDto(GameTable gameTable);

    public partial GameTableDto ToGameTableDto(GameTable gameTable);

    public partial GameTable ToGameTable(NewDto dto);

    public partial GameTableOperation ToGameTableOperation(GameTableOperationDto dto);
}