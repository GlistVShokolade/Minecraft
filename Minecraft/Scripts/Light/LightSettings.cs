using OpenTK.Mathematics;

public struct LightSettings
{
    public Vector4 Position;
    public Vector4 Direction;

    public LightSettings(Vector4 position, Vector4 direction)
    {
        Position = position;
        Direction = direction;
    }
}
