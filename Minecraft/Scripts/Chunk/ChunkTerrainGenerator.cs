using OpenTK.Mathematics;

public class ChunkTerrainGenerator
{
    public static ChunkTerrainGenerator Instance;

    private FastNoiseLite _terrainNoise;
    private FastNoiseLite _caveNoise;

    private readonly NoiseSettings _terrainSettings;
    private readonly NoiseSettings _caveSettings;

    public ChunkTerrainGenerator(NoiseSettings terrainSettings, NoiseSettings caveSettings)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetNoise(terrainSettings, ref _terrainNoise);
        SetNoise(caveSettings, ref _caveNoise);

        _terrainSettings = terrainSettings;
        _caveSettings = terrainSettings;
    }

    private void SetNoise(NoiseSettings settings, ref FastNoiseLite noise)
    {
        noise = new FastNoiseLite();

        noise.SetNoiseType(settings.NoiseType);
        noise.SetFrequency(settings.Frequency);
        noise.SetFractalType(settings.FractalType);
        noise.SetRotationType3D(settings.RotationType);
    }

    public BlockType[,,] Generate(Vector3i chunkPosition)
    {
        var blocks = new BlockType[Chunk.Width, Chunk.Height, Chunk.Width];

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int z = 0; z < Chunk.Width; z++)
            {
                float terrainHeight = _terrainNoise.GetNoise(
                    (x + chunkPosition.X) * _terrainSettings.Amplitude,
                    (z + chunkPosition.Z) * _terrainSettings.Amplitude) * _terrainSettings.Depth + 32;

                for (int y = 0; y < terrainHeight; y++)
                {
                    if (y == (int)terrainHeight)
                    {
                        blocks[x, y, z] = BlockType.Grass;
                    }
                    else if ((int)terrainHeight - y < (int)terrainHeight)
                    {
                        blocks[x, y, z] = BlockType.Stone;
                    }
                }

                for (int y = 0; y < Chunk.Height; y++)
                {
                    float caveWeight = _caveNoise.GetNoise((x + chunkPosition.X) * _caveSettings.Amplitude, y * _caveSettings.Amplitude, (z + chunkPosition.Z) * _caveSettings.Amplitude) * _caveSettings.Depth;

                    if (caveWeight >= 1.5f)
                    {
                        blocks[x, y, z] = BlockType.Air;
                    }
                }
            }
        }

        return blocks;
    }
}
