using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.Template;
using Arpg.Core.Interfaces.Security;
using Arpg.Core.Models.Customer;
using Arpg.Core.Models.Definitions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Arpg.Server.Tests;

public class TemplateServicesTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ITemplateRepository> _templateRepositoryMock;
    private readonly Mock<ISheetQueries> _sheetRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IValidator<NewTemplateDto>> _createDtoValidatorMock;
    private readonly Mock<IValidator<EditTemplateDto>> _editDtoValidatorMock;
    private readonly Mock<IValidator<DeleteTemplateDto>> _deleteDtoValidatorMock;

    private readonly TemplateServices _sut;

    public TemplateServicesTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _templateRepositoryMock = new Mock<ITemplateRepository>();
        _sheetRepositoryMock = new Mock<ISheetQueries>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _createDtoValidatorMock = new Mock<IValidator<NewTemplateDto>>();
        _editDtoValidatorMock = new Mock<IValidator<EditTemplateDto>>();
        _deleteDtoValidatorMock = new Mock<IValidator<DeleteTemplateDto>>();

        _sut = new TemplateServices(
            _userContextMock.Object,
            _passwordHasherMock.Object,
            _accountRepositoryMock.Object,
            _templateRepositoryMock.Object,
            _sheetRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _createDtoValidatorMock.Object,
            _editDtoValidatorMock.Object,
            _deleteDtoValidatorMock.Object
        );
    }

    [Fact]
    public async Task DeleteAsync_WithWrongPassword_ReturnsUnprocessableEntityError()
    {
        var dto = new DeleteTemplateDto(Guid.NewGuid(), "wrongpass");
        var userId = Guid.NewGuid();
        var account = new Account(userId, "test@test.com");

        _userContextMock.Setup(x => x.Id).Returns(userId);
        _deleteDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());
        _accountRepositoryMock.Setup(x => x.GetOwnerAsync(userId)).ReturnsAsync(account);
        _passwordHasherMock.Setup(x => x.Verify("wrongpass", It.IsAny<string>())).Returns(false);

        var result = await _sut.DeleteAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid password.");
        _templateRepositoryMock.Verify(x => x.Delete(It.IsAny<Template>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_TemplateHasSheets_ArchivesTemplateInsteadOfDeleting()
    {
        var templateId = Guid.NewGuid();
        var dto = new DeleteTemplateDto(templateId, "correctpass");
        var userId = Guid.NewGuid();
        var account = new Account(userId, "test@test.com");
        var template = new Template { Id = templateId, OwnerId = userId, Name = "Original Name" };

        _userContextMock.Setup(x => x.Id).Returns(userId);
        _deleteDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());
        _accountRepositoryMock.Setup(x => x.GetOwnerAsync(userId)).ReturnsAsync(account);
        _passwordHasherMock.Setup(x => x.Verify("correctpass", It.IsAny<string>())).Returns(true);
        _templateRepositoryMock.Setup(x => x.GetAsync(templateId, userId)).ReturnsAsync(template);
        _sheetRepositoryMock.Setup(x => x.AnyByTemplate(templateId)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(dto);

        result.IsSuccess.Should().BeTrue();

        // Assert it was NOT physically deleted
        _templateRepositoryMock.Verify(x => x.Delete(It.IsAny<Template>()), Times.Never);

        // Assert it was Archived
        template.IsArchived.Should().BeTrue();
        template.OwnerId.Should().Be(Guid.Empty);
        template.Name.Should().Be("Deleted template");

        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_TemplateHasNoSheets_PhysicallyDeletesTemplate()
    {
        var templateId = Guid.NewGuid();
        var dto = new DeleteTemplateDto(templateId, "correctpass");
        var userId = Guid.NewGuid();
        var account = new Account(userId, "test@test.com");
        var template = new Template { Id = templateId, OwnerId = userId };

        _userContextMock.Setup(x => x.Id).Returns(userId);
        _deleteDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());
        _accountRepositoryMock.Setup(x => x.GetOwnerAsync(userId)).ReturnsAsync(account);
        _passwordHasherMock.Setup(x => x.Verify("correctpass", It.IsAny<string>())).Returns(true);
        _templateRepositoryMock.Setup(x => x.GetAsync(templateId, userId)).ReturnsAsync(template);
        _sheetRepositoryMock.Setup(x => x.AnyByTemplate(templateId)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(dto);

        result.IsSuccess.Should().BeTrue();

        // Assert it WAS physically deleted
        _templateRepositoryMock.Verify(x => x.Delete(template), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
