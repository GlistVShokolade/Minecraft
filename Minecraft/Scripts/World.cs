public class World
{
    private readonly ChunkSpawner _spawner;
    private readonly ChunkLoader _loader;

    public World(Transform player)
    {
        _loader = new ChunkLoader(player);
        _spawner = new ChunkSpawner(_loader);
    }
}
