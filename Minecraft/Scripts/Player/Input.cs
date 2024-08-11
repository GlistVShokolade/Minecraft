using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Input
{
    private static  KeyboardState? _keyboard;
    private static MouseState? _mouse;

    public static Vector2 MousePosition => _mouse.Position;

    public Input(KeyboardState keyboard, MouseState mouse)
    {
        _keyboard = keyboard ?? throw new NullReferenceException(nameof(keyboard));
        _mouse = mouse ?? throw new NullReferenceException(nameof(mouse));
    }

    public static bool GetMouseDown(MouseButton key)
    {
        return _mouse.IsButtonDown(key);
    }

    public static bool IsKeyPressed(Keys key)
    {
        return _keyboard.IsKeyPressed(key);
    }

    public static bool IsKeyReleased(Keys key)
    {
        return _keyboard.IsKeyReleased(key);
    }
    public static bool IsKeyDown(Keys key)
    {
        return _keyboard.IsKeyDown(key);
    }
}
