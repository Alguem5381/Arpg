using Arpg.Application.Abstractions;
using Arpg.Application.Auth;
using Arpg.Application.Repositories;
using Arpg.Contracts.Dto.User;
using Arpg.Core.Interfaces.Security;
using Arpg.Core.Models.Customer;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Arpg.Server.Tests;

public class AccountServicesTests
{
    private readonly Mock<ITokenServices> _tokenServicesMock = new();
    private readonly Mock<IEmailServices> _emailServicesMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ICodeRepository> _codeRepositoryMock = new();
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IValidator<NewUserDto>> _newDtoValidatorMock = new();
    private readonly Mock<IValidator<LoginDto>> _loginDtoValidatorMock = new();
    private readonly Mock<IValidator<ValidateCodeDto>> _validateCodeDtoValidatorMock = new();
    private readonly Mock<IValidator<DeleteUserDto>> _deleteDtoValidatorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AccountServices _sut;

    public AccountServicesTests()
    {
        _sut = new AccountServices(
            _tokenServicesMock.Object,
            _emailServicesMock.Object,
            _userContextMock.Object,
            _userRepositoryMock.Object,
            _codeRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _passwordHasherMock.Object,
            _newDtoValidatorMock.Object,
            _loginDtoValidatorMock.Object,
            _validateCodeDtoValidatorMock.Object,
            _deleteDtoValidatorMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WithLockedOutAccount_ReturnsUnauthorizedWithoutCheckingPassword()
    {
        // Arrange
        var dto = new LoginDto("testuser", "anypassword");
        _loginDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());

        var account = new Account(Guid.NewGuid(), "test@test.com");

        // Simular lockout gravando falhas
        for (int i = 0; i < 5; i++) account.RecordFailedLogin();

        _accountRepositoryMock.Setup(x => x.GetAsync(dto.Username)).ReturnsAsync(account);

        // Act
        var result = await _sut.LoginAsync(dto);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e =>
            e.Message == "Account is temporarily locked out due to multiple failed login attempts.");

        // Password shouldn't even be hashed or verified
        _passwordHasherMock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPasswordNotLockedOut_RecordsFailedLoginAndReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto("testuser", "wrongpassword");
        _loginDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());

        var account = new Account(Guid.NewGuid(), "test@test.com");
        _passwordHasherMock.Setup(x => x.Hash("correctpassword")).Returns("hashedpassword");
        account.SetInitialPassword("correctpassword", _passwordHasherMock.Object);

        _accountRepositoryMock.Setup(x => x.GetAsync(dto.Username)).ReturnsAsync(account);
        _passwordHasherMock.Setup(x => x.Verify("wrongpassword", "hashedpassword")).Returns(false);

        // Act
        var result = await _sut.LoginAsync(dto);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid credentials");

        account.FailedLoginAttempts.Should().Be(1);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_SendsEmailAndReturnsCode()
    {
        // Arrange
        var dto = new LoginDto("testuser", "correctpassword");
        _loginDtoValidatorMock.Setup(x => x.Validate(dto)).Returns(new ValidationResult());

        var account = new Account(Guid.NewGuid(), "test@test.com");
        _passwordHasherMock.Setup(x => x.Hash("correctpassword")).Returns("hashedpassword");
        account.SetInitialPassword("correctpassword", _passwordHasherMock.Object);

        _accountRepositoryMock.Setup(x => x.GetAsync(dto.Username)).ReturnsAsync(account);
        _passwordHasherMock.Setup(x => x.Verify("correctpassword", "hashedpassword")).Returns(true);

        // Act
        var result = await _sut.LoginAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _emailServicesMock.Verify(x => x.SendCodeVerificationEmailAsync(account.Email, It.IsAny<string>()), Times.Once);
    }
}
