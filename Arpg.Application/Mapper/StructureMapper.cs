using Arpg.Contracts.Dto.Structure;
using Arpg.Core.Models.Batch;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class StructureMapper
{
    public partial BatchUpdate BatchUpdateDtoToBatchUpdate(BatchUpdateDto batchUpdateDto);
}