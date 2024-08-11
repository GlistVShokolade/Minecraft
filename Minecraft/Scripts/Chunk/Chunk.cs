public class Chunk : GameObject
{
    public const byte Width = 16;
    public const byte Height = 128;

    public Chunk(Transform transform, Renderer renderer) : base(transform, renderer)
    {
    }
}
