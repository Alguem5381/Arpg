using Arpg.Contracts.Dto.User;
using Arpg.Core.Models.Customer;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class AccountMapper
{
    [MapperIgnoreSource(nameof(Code.OwnerId))]
    [MapperIgnoreSource(nameof(Code.Value))]
    public partial CodeDto CodeToCodeDto(Code code);
}