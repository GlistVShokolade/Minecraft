using OpenTK.Mathematics;

public class ChunkLoader : IUpdatable
{
    private readonly Transform _player;

    private Vector2i _lastChunkPosition = Vector2i.UnitY;
    private Vector2i PlayerChunkPosition => new Vector2i((int)_player.Position.X, (int)_player.Position.Z) / Chunk.Width;

    private readonly Dictionary<Vector2i, LoadedChunkData> _loadedChunks = new Dictionary<Vector2i, LoadedChunkData>();

    public event Action<LoadedChunkData[]> ChunksLoaded;

    public ChunkLoader(Transform player)
    {
        _player = player ?? throw new NullReferenceException(nameof(player));

        GlobalUpdator.RegisterUpdate(this);
    }

    ~ChunkLoader()
    {
        GlobalUpdator.UnregisterUpdate(this);
    }

    public void LoadChunks()
    {
        var tempChunkBuffer = new List<LoadedChunkData>();

        var currentChunkPosition = PlayerChunkPosition;

        for (int x = currentChunkPosition.X - Camera.ViewRadius; x < currentChunkPosition.X + Camera.ViewRadius; x++)
        {
            for (int y = currentChunkPosition.Y - Camera.ViewRadius; y < currentChunkPosition.Y + Camera.ViewRadius; y++)
            {
                var localPosition = new Vector2i(x, y); 

                if (_loadedChunks.ContainsKey(localPosition))
                {
                    continue;
                }

                var distance = Vector2.Distance(currentChunkPosition, localPosition);
                
                if (distance > Camera.ViewRadius)
                {
                    continue;
                }

                var worldPosition = new Vector3i(x, 0, y) * Chunk.Width;

                var loadedData = LoadChunk(worldPosition, localPosition);

                tempChunkBuffer.Add(loadedData);
                _loadedChunks.Add(localPosition, loadedData);
            }
        }

        ChunksLoaded?.Invoke(tempChunkBuffer.ToArray());
    }

    private LoadedChunkData LoadChunk(Vector3i worldPosition, Vector2i localPosition)
    {
        var loadedChunkData = new LoadedChunkData();

        var blocks = ChunkTerrainGenerator.Instance.Generate(worldPosition);
        var mesh = ChunkMeshBuilder.Instance.Generate(blocks, worldPosition);

        loadedChunkData.Mesh = mesh;
        loadedChunkData.WorldPosition = worldPosition;
        loadedChunkData.LocalPosition = localPosition;

        return loadedChunkData;
    }

    public void Update()
    {
        TryLoad();
    }

    private bool TryLoad()
    {
        var currentChunkPosition = PlayerChunkPosition;

        if (_lastChunkPosition != currentChunkPosition)
        {
            _lastChunkPosition = currentChunkPosition;

            LoadChunks();

            return true;
        }

        return false;
    }
}
