using Arpg.Core.Interfaces.Security;

namespace Arpg.Core.Models.Customer;

public class Account
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }

    public bool IsValid { get; set; }
    public string PasswordHash { get; private set; } = string.Empty;
    
    public List<Code> Codes { get; } = [];
    

    public Account() { }
    public Account(Guid userId)
    {
        UserId = userId;
    }

    public void SetInitialPassword(string password, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(password)) return;

        if (PasswordHash == string.Empty)
            PasswordHash = hasher.Hash(password);
    }

    public bool PasswordMatches(string password, IPasswordHasher hasher)
    {
        return hasher.Verify(password, PasswordHash);
    }

    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        int minutesToLock = (int)Math.Pow(2, FailedLoginAttempts); // 2, 4, 8, 16, 32... minutes
        LockoutEnd = DateTime.UtcNow.AddMinutes(minutesToLock);
    }

    public void ResetFailedLogins()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }
}