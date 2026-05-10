using Arpg.Contracts.Dto.Structure;
using Arpg.Core.Models.Definitions.Batch;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class StructureMapper
{
    public partial BatchUpdate BatchUpdateDtoToBatchUpdate(BatchStructureDto batchUpdateDto);
}