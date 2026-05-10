using System.IO;
using Arpg.Client.Abstractions;

namespace Arpg.Client.Services;

public class UserSession : IUserSession
{
    private readonly string _tokenFilePath;
    private string? _token;

    public UserSession()
    {
        _tokenFilePath = Path.Combine(Path.GetTempPath(), "arpg_client_token.tmp");
        if (File.Exists(_tokenFilePath))
            _token = File.ReadAllText(_tokenFilePath);
    }

    public string? Token
    {
        get => _token;
        set
        {
            _token = value;
            if (string.IsNullOrEmpty(_token))
            {
                if (File.Exists(_tokenFilePath))
                    File.Delete(_tokenFilePath);
            }
            else
                File.WriteAllText(_tokenFilePath, _token);
        }
    }
}
