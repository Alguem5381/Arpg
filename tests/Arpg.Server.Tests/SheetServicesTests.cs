using Arpg.Application.Auth;
using Arpg.Application.Repositories;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.Sheet;
using Arpg.Core.Models;
using Arpg.Core.Models.Definitions;
using Arpg.Primitives.Enums.Template;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Arpg.Server.Tests;

public class SheetServicesTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ISheetRepository> _sheetRepositoryMock;
    private readonly Mock<ITemplateRepository> _templateRepositoryMock;
    private readonly Mock<IValidator<CreateDto>> _createValidatorMock;
    private readonly Mock<IValidator<EditDto>> _editValidatorMock;
    private readonly Mock<IValidator<ComputeDto>> _computeValidatorMock;

    private readonly SheetServices _sut;

    public SheetServicesTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _sheetRepositoryMock = new Mock<ISheetRepository>();
        _templateRepositoryMock = new Mock<ITemplateRepository>();
        _createValidatorMock = new Mock<IValidator<CreateDto>>();
        _editValidatorMock = new Mock<IValidator<EditDto>>();
        _computeValidatorMock = new Mock<IValidator<ComputeDto>>();

        _sut = new SheetServices(
            _unitOfWorkMock.Object,
            _userContextMock.Object,
            _sheetRepositoryMock.Object,
            _templateRepositoryMock.Object,
            _createValidatorMock.Object,
            _editValidatorMock.Object,
            _computeValidatorMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_TemplateNotFound_ReturnsNotFoundError()
    {
        var dto = new CreateDto("My Sheet", Guid.NewGuid());
        _createValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _templateRepositoryMock.Setup(x => x.GetAsync(dto.TemplateId)).ReturnsAsync((Template?)null);

        var result = await _sut.CreateAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Template not found.");
    }

    [Fact]
    public async Task CreateAsync_ValidTemplate_PopulatesSheetAndCommits()
    {
        var templateId = Guid.NewGuid();
        var fieldId = Guid.NewGuid();
        var dto = new CreateDto("My Sheet", templateId);
        var userId = Guid.NewGuid();
        
        var template = new Template { Id = templateId };
        template.Structure.Categories.Add(new TemplateCategory { Id = Guid.NewGuid() });
        template.Structure.Fields.Add(new TemplateField { Id = fieldId, CategoryId = Guid.NewGuid(), Type = FieldType.Text, DefaultValue = "Default" });

        _createValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _templateRepositoryMock.Setup(x => x.GetAsync(templateId)).ReturnsAsync(template);
        _userContextMock.Setup(x => x.Id).Returns(userId);

        var result = await _sut.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        
        _sheetRepositoryMock.Verify(x => x.Add(It.Is<Sheet>(s => 
            s.Name == "My Sheet" && 
            s.OwnerId == userId && 
            s.TemplateId == templateId &&
            s.Data.ContainsKey(fieldId))), Times.Once);
            
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ComputeDataAsync_SheetNotFound_ReturnsNotFoundError()
    {
        var dto = new ComputeDto(Guid.NewGuid(), new Dictionary<Guid, object?>());
        var userId = Guid.NewGuid();

        _computeValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _sheetRepositoryMock.Setup(x => x.GetSheetAsync(dto.Id, userId)).ReturnsAsync((Sheet?)null);

        var result = await _sut.ComputeDataAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Sheet not found.");
    }

    [Fact]
    public async Task ComputeDataAsync_ValidData_ComputesAndCommits()
    {
        var sheetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var fieldId = Guid.NewGuid();
        
        var dto = new ComputeDto(sheetId, new Dictionary<Guid, object?> { { fieldId, 100 } });
        var sheet = new Sheet { Id = sheetId, OwnerId = userId, TemplateId = templateId };
        var template = new Template { Id = templateId };
        template.Structure.Fields.Add(new TemplateField { Id = fieldId, Type = FieldType.Number });

        _computeValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _sheetRepositoryMock.Setup(x => x.GetSheetAsync(sheetId, userId)).ReturnsAsync(sheet);
        _templateRepositoryMock.Setup(x => x.GetAsync(templateId)).ReturnsAsync(template);

        var result = await _sut.ComputeDataAsync(dto);

        result.IsSuccess.Should().BeTrue();
        sheet.Data.Should().ContainKey(fieldId).WhoseValue.Should().Be(100);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidSheet_DeletesFromRepositoryAndCommits()
    {
        var sheetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sheet = new Sheet { Id = sheetId, OwnerId = userId };

        _userContextMock.Setup(x => x.Id).Returns(userId);
        _sheetRepositoryMock.Setup(x => x.GetSheetAsync(sheetId, userId)).ReturnsAsync(sheet);

        var result = await _sut.DeleteAsync(sheetId);

        result.IsSuccess.Should().BeTrue();
        _sheetRepositoryMock.Verify(x => x.Delete(sheet), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
