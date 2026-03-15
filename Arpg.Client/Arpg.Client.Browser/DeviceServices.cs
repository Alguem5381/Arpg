using System.Runtime.InteropServices.JavaScript;
using Arpg.Client.Abstractions;

namespace Arpg.Client.Browser;

public partial class DeviceServices : IDeviceServices
{
    [JSImport("globalThis.ArpgInterop.getScreenWidth")]
    private static partial int GetScreenWidth();

    [JSImport("globalThis.ArpgInterop.IsMobileDevice")]
    private static partial bool IsMobileDevice();
    
    public int ScreenWidth => GetScreenWidth();
    public bool IsMobile => IsMobileDevice();
}