using Arpg.Client.Abstractions;

namespace Arpg.Client.Desktop;

public class DeviceServices : IDeviceServices
{
    public bool IsMobile { get; } = false;
    public int ScreenWidth => 0;
}