public class FPSCounter
{
    private ushort _currentFPS;
    private double _frameTime;

    public event Action<ushort> FPSCalculated;

    public void Calculate()
    {
        if (_frameTime < 1d)
        {   
            _frameTime += Time.Instance.UpdateTime;
            _currentFPS++;
        }
        else
        {
            FPSCalculated?.Invoke(_currentFPS);

            _frameTime = 0d;
            _currentFPS = 0;
        }
    }
}
