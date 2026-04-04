namespace Arpg.Client.Abstractions;

public interface IDeviceServices
{
    int ScreenWidth { get; }
    bool IsMobile { get; }
}