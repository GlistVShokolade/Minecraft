using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public class Program
{
    private static void Main(string[] args)
    {
        var gameSettings = new GameWindowSettings();
        var nativeSettings = new NativeWindowSettings();

        nativeSettings.ClientSize = new Vector2i(1280, 720);
        nativeSettings.Vsync = VSyncMode.On;
        nativeSettings.Profile = ContextProfile.Compatability;

        using (var window = new Window(gameSettings, nativeSettings))
        {
            window.Run();
        }
    }
}
