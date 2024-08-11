using OpenTK.Mathematics;

public class ChunkSpawner
{
    private Dictionary<Vector2i, Chunk> _spawnedChunks = new Dictionary<Vector2i, Chunk>();

    private readonly ChunkLoader _chunkLoader;
    
    public ChunkSpawner(ChunkLoader chunkLoader)
    {
        _chunkLoader = chunkLoader ?? throw new NullReferenceException(nameof(chunkLoader));

        _chunkLoader.ChunksLoaded += SpawnChunks;
    }

    ~ChunkSpawner()
    {
        _chunkLoader.ChunksLoaded -= SpawnChunks;
    }

    private void SpawnChunks(LoadedChunkData[] loadedChunkDatas)
    {
        for (int i = 0; i < loadedChunkDatas.Length; i++)
        {
            SpawnChunk(loadedChunkDatas[i]);
        }
    }

    private void SpawnChunk(LoadedChunkData loadedChunkData)
    {
        var transform = new Transform(loadedChunkData.WorldPosition, Vector3.One, Vector3.Zero);
        var renderer = new MeshRenderer(ref loadedChunkData.Mesh);

        var chunk = new Chunk(transform, renderer);

        _spawnedChunks.Add(loadedChunkData.LocalPosition, chunk);
    }

    public Chunk GetChunk(Vector2i chunkPosition)
    {
        return _spawnedChunks[chunkPosition];
    }
}
