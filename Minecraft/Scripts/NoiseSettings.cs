public struct NoiseSettings
{
    public float Amplitude { get; }
    public float Frequency { get; }
    public float Depth { get; }
    public FastNoiseLite.NoiseType NoiseType { get; }
    public FastNoiseLite.FractalType FractalType { get; }
    public FastNoiseLite.RotationType3D RotationType { get; }

    public NoiseSettings(float amplitude, float frequency, float depth, FastNoiseLite.NoiseType noiseType, FastNoiseLite.FractalType fractalType, FastNoiseLite.RotationType3D rotationType)
    {
        Depth = depth;
        Amplitude = amplitude;
        Frequency = frequency;
        NoiseType = noiseType;
        FractalType = fractalType;
        RotationType = rotationType;
    }
}
