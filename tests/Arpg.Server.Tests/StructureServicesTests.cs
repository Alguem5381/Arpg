using Arpg.Application.Auth;
using Arpg.Application.Repositories;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.Structure;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Enums.Template;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Arpg.Server.Tests;

public class StructureServicesTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ITemplateRepository> _templateRepositoryMock;
    
    private readonly StructureServices _sut;

    public StructureServicesTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _templateRepositoryMock = new Mock<ITemplateRepository>();

        _sut = new StructureServices(
            _unitOfWorkMock.Object,
            _userContextMock.Object,
            _templateRepositoryMock.Object
        );
    }

    [Fact]
    public async Task UpdateStructureAsync_TemplateNotFound_ReturnsNotFoundError()
    {
        var dto = new BatchUpdateDto();
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _templateRepositoryMock.Setup(x => x.GetAsync(dto.TemplateId, userId)).ReturnsAsync((Template?)null);

        var result = await _sut.UpdateStructureAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Template not found.");
    }

    [Fact]
    public async Task UpdateStructureAsync_ValidOperations_UpdatesStructureAndCommits()
    {
        var categoryId = Guid.NewGuid();
        var dto = new BatchUpdateDto
        {
            TemplateId = Guid.NewGuid(),
            Categories = new List<CategoryOpDto>
            {
                new CategoryOpDto { Id = categoryId, Op = Operation.Add, Name = "Basic Category", Order = 1 }
            },
            Fields = new List<FieldOpDto>
            {
                new FieldOpDto { Id = Guid.NewGuid(), CategoryId = categoryId, Op = Operation.Add, Name = "Strength", Type = FieldType.Number }
            }
        };
        
        var userId = Guid.NewGuid();
        var template = new Template { Id = dto.TemplateId, OwnerId = userId };
        
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _templateRepositoryMock.Setup(x => x.GetAsync(dto.TemplateId, userId)).ReturnsAsync(template);

        var result = await _sut.UpdateStructureAsync(dto);

        result.IsSuccess.Should().BeTrue();
        
        // Assert that the internal core logic of TemplateStructure worked properly via the Domain Models.
        template.Structure.Categories.Should().ContainSingle(c => c.Name == "Basic Category");
        template.Structure.Fields.Should().ContainSingle(f => f.Name == "Strength");
        
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
