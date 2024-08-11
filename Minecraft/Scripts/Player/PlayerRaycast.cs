using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class PlayerRaycast : IUpdatable
{
    private const float RAY_STEP = 0.01f;

    private readonly ChunkSpawner _spawner;
    private readonly Camera _camera;

    public PlayerRaycast(ChunkSpawner chunkSpawner, Camera camera)
    {
        _spawner = chunkSpawner ?? throw new NullReferenceException(nameof(chunkSpawner));
        _camera = camera ?? throw new NullReferenceException(nameof(camera));

        GlobalUpdator.RegisterUpdate(this);
    }

    ~PlayerRaycast()
    {
        GlobalUpdator.UnregisterUpdate(this);
    }

    public void Update()
    {
        if (Input.GetMouseDown(MouseButton.Left) == false)
        {
            return;
        }
        
    }
}