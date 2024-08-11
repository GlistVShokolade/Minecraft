using OpenTK.Windowing.Desktop;

public class Time
{
    public static Time Instance;

    private readonly GameWindow _window;

    public float UpdateTime => (float)_window.UpdateTime;

    public Time(GameWindow window)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _window = window ?? throw new NullReferenceException(nameof(window));
    }
}
