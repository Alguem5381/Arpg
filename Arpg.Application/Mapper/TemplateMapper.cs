using Arpg.Contracts.Dto.Template;
using Arpg.Core.Models;
using Riok.Mapperly.Abstractions;

namespace Arpg.Application.Mapper;

[Mapper]
public partial class TemplateMapper
{
    public partial TemplateDto TemplateToTemplateDto(Template template);
}