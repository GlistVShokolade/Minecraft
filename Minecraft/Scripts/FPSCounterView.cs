using OpenTK.Windowing.Desktop;

public class FPSCounterView
{
    private readonly FPSCounter _counter;
    private readonly GameWindow _window;

    public FPSCounterView(FPSCounter counter, GameWindow window)
    {
        _counter = counter ?? throw new NullReferenceException(nameof(counter));
        _window = window ?? throw new NullReferenceException(nameof(window));

        _counter.FPSCalculated += UpdateValue;
    }

    ~FPSCounterView()
    {
        _counter.FPSCalculated -= UpdateValue;
    }

    private void UpdateValue(ushort value)
    {
        _window.Title = value.ToString();
    }
}
