using Arpg.Application.Auth;
using Arpg.Application.Repositories;
using Arpg.Application.Services;
using Arpg.Contracts.Dto.User;
using Arpg.Core.Models.Customer;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Arpg.Server.Tests;

public class UserServicesTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IValidator<EditDto>> _editDtoValidatorMock;
    
    private readonly UserServices _sut;

    public UserServicesTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userContextMock = new Mock<IUserContext>();
        _editDtoValidatorMock = new Mock<IValidator<EditDto>>();

        _sut = new UserServices(
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _userContextMock.Object,
            _editDtoValidatorMock.Object
        );
    }

    [Fact]
    public async Task EditAsync_WithInvalidDto_ReturnsValidationFailure()
    {
        var dto = new EditDto("A");
        var validationResult = new ValidationResult(new[] { new ValidationFailure("DisplayName", "Too short") });
        _editDtoValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(validationResult);

        var result = await _sut.EditAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Too short");
    }

    [Fact]
    public async Task EditAsync_UserNotFound_ReturnsNotFoundError()
    {
        var dto = new EditDto("New Name");
        var userId = Guid.NewGuid();
        
        _editDtoValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _userRepositoryMock.Setup(x => x.GetAsync(userId)).ReturnsAsync((User?)null);

        var result = await _sut.EditAsync(dto);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "User not found.");
    }

    [Fact]
    public async Task EditAsync_ValidRequest_UpdatesUserAndReturnsDto()
    {
        var dto = new EditDto("New Display Name");
        var user = new User { DisplayName = "Old Name", Username = "user123" };
        var userId = user.Id;
        
        _editDtoValidatorMock.Setup(x => x.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
        _userContextMock.Setup(x => x.Id).Returns(userId);
        _userRepositoryMock.Setup(x => x.GetAsync(userId)).ReturnsAsync(user);

        var result = await _sut.EditAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.DisplayName.Should().Be("New Display Name");
        result.Value.Username.Should().Be("user123");
        
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
