using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;


public class Camera : Transform
{
    public const float FOV = 90f;
    public const float MaxPitchRotation = 89.9f;

    public const float ScreenAspect = 16f / 10f;
    public const float ZFar = 1000f;
    public const float ZNear = 0.001f;

    public const int ViewRadius = 16;
    private bool _firstRotate = true;

    private float _yaw = -90f;
    private float _pitch;

    private Vector2 _lastPosition = Vector2.Zero;

    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    private readonly float _sensivity;
    private readonly float _moveSpeed;

    public Camera(float moveSpeed, float sensivity, Vector3 position, Vector3 scale, Quaternion rotation) : base(position, scale, rotation)
    {
        _moveSpeed = moveSpeed >= 0f ? moveSpeed : throw new ArgumentOutOfRangeException(nameof(moveSpeed));
        _sensivity = sensivity >= 0f ? sensivity : throw new ArgumentOutOfRangeException(nameof(sensivity));

        TransformChanged += LoadViewMatrix;

        LoadViewMatrix();
        LoadPerspectiveMatrix();
    }

    ~Camera()
    {
        TransformChanged -= LoadViewMatrix;
    }

    private void LoadViewMatrix()
    {
        var view = Matrix4.LookAt(Position, Position + _front, _up);

        GL.MatrixMode(MatrixMode.Modelview);

        GL.LoadIdentity();
        GL.LoadMatrix(ref view);
    }

    public void Move()
    {
        var input = Input.Instance;

        Vector3 direction = Vector3.Zero;

        if (input.IsKeyDown(Keys.W))
        {
            direction += _front;
        }
        if (input.IsKeyDown(Keys.S))
        {
            direction -= _front;
        }
        if (input.IsKeyDown(Keys.A))
        {
            direction -= _right;
        }
        if (input.IsKeyDown(Keys.D))
        {
            direction += _right;
        }
        if (input.IsKeyDown(Keys.Space))
        {
            direction += _up;
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            direction -= _up;
        }

        direction *= _moveSpeed * Time.Instance.UpdateTime;

        Move(direction);
    }

    public void Rotate()
    {
        var currentPosition = Input.Instance.MousePosition;

        if (currentPosition == _lastPosition)
        {
            return;
        }

        if (_firstRotate)
        {
            _lastPosition = new Vector2(currentPosition.X, currentPosition.Y);
            _firstRotate = false;
        }
        else
        {
            float deltaX = currentPosition.X - _lastPosition.X;
            float deltaY = currentPosition.Y - _lastPosition.Y;

            _lastPosition = new Vector2(currentPosition.X, currentPosition.Y);

            _yaw += deltaX * _sensivity;
            _pitch -= deltaY * _sensivity;

            _pitch = Math.Clamp(_pitch, -MaxPitchRotation, MaxPitchRotation);
        }

        RecalculateVectors();
        LoadViewMatrix();
    }

    private void LoadPerspectiveMatrix()
    {
        GL.MatrixMode(MatrixMode.Projection);

        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), ScreenAspect, ZNear, ZFar);

        GL.LoadIdentity();
        GL.LoadMatrix(ref projection);
    }

    private void RecalculateVectors()
    {
        _front.X = (float)(Math.Cos(MathHelper.DegreesToRadians(_pitch)) * Math.Cos(MathHelper.DegreesToRadians(_yaw)));
        _front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
        _front.Z = (float)(Math.Cos(MathHelper.DegreesToRadians(_pitch)) * Math.Sin(MathHelper.DegreesToRadians(_yaw)));

        _front = Vector3.Normalize(_front);

        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }
}
