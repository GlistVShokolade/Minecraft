using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Transform
{
    public Vector3 Position { get; private set; }
    public Vector3 Scale { get; private set; }
    public Quaternion Rotation { get; private set; }

    public event Action TransformChanged;

    public Transform(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        TransformChanged += LoadModelMatrix;

        SetPosition(position);
        SetRotation(rotation);
        SetScale(scale);
    }

    ~Transform()
    {
        TransformChanged -= LoadModelMatrix;
    }

    private void LoadModelMatrix()
    {
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

        var angleX = MathHelper.DegreesToRadians(Rotation.X);
        var angleY = MathHelper.DegreesToRadians(Rotation.Y);
        var angleZ = MathHelper.DegreesToRadians(Rotation.Z);

        var modelMatrix =
            Matrix4.CreateTranslation(Position) *
            Matrix4.CreateRotationX(angleX) *
            Matrix4.CreateRotationY(angleY) *
            Matrix4.CreateRotationZ(angleZ) *
            Matrix4.CreateScale(Scale);

        GL.LoadMatrix(ref modelMatrix);
    }

    public void SetRotation(Quaternion rotation)
    {
        if (Rotation == rotation)
        {
            return;
        }

        Rotation = rotation;

        TransformChanged?.Invoke();
    }

    public void SetScale(Vector3 scale)
    {
        if (scale.X < 0f || scale.Y < 0f || scale.Z < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        Scale = scale;

        TransformChanged?.Invoke();
    }

    public void SetPosition(Vector3 position)
    {
        if (Position == position)
        {
            return;
        }

        Position = position;

        TransformChanged?.Invoke();
    }

    public void Move(Vector3 velocity)
    {
        if (velocity == Vector3.Zero)
        {
            return;
        }

        Position += velocity;

        TransformChanged?.Invoke();
    }
}