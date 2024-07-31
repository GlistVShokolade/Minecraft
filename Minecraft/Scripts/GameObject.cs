using OpenTK.Mathematics;

public abstract class GameObject
{
    public Transform Transform { get; private set; }
    public Renderer Renderer { get; private set; }

    public GameObject(Transform transform, Renderer renderer)
    {
        Transform = transform;
        Renderer = renderer;
    }

    public GameObject(Renderer renderer)
    {
        Renderer = renderer;
        Transform = new Transform(Vector3.Zero, Vector3.One, Quaternion.Identity);
    }
}
