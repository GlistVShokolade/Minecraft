using OpenTK.Mathematics;

public abstract class GameObject
{
    public Transform Transform { get; }
    public Renderer Renderer { get; }

    public GameObject(Transform transform, Renderer renderer)
    {
        Transform = transform;
        Renderer = renderer;
    }

    public GameObject(Renderer renderer)
    {
        Renderer = renderer;
        Transform = new Transform(Vector3.Zero, Vector3.One, Vector3.Zero);
    }
}
