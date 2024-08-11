using OpenTK.Mathematics;

public class ChunkMeshBuilder
{
    public static ChunkMeshBuilder Instance;

    private List<Polygon> _polygons;

    private Vector3i _chunkPosition;
    private BlockType[,,] _blocks;

    public ChunkMeshBuilder()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Mesh Generate(BlockType[,,] blocks, Vector3i chunkPosition)
    {
        _polygons = new List<Polygon>();

        _blocks = blocks;
        _chunkPosition = chunkPosition;

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int z = 0; z < Chunk.Width; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        var polygons = _polygons.ToArray();

        _blocks = null;
        _polygons = null;

        return new Mesh(polygons);
    }

    private void GenerateBlock(int x, int y, int z)
    {
        if (_blocks[x, y, z] == BlockType.Air)
        {
            return;
        }

        var block = BlockContainer.Get(_blocks[x, y, z]);
        var blockPosition = new Vector3i(x, y, z);

        if (GetBlock(blockPosition + Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateBackFace(block, blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateFrontFace(block, blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitX) == BlockType.Air)
        {
            GenerateRightFace(block, blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitX) == BlockType.Air)
        {
            GenerateLeftFace(block, blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitY) == BlockType.Air)
        {
            GenerateTopFace(block, blockPosition + _chunkPosition);
        }
        if (blockPosition.Y > 0 && GetBlock(blockPosition - Vector3i.UnitY) == BlockType.Air)
        {
            GenerateBottomFace(block, blockPosition + _chunkPosition);
        }
    }

    private BlockType GetBlock(Vector3i blockPosition)
    {
        if (blockPosition.X >= 0 && blockPosition.X < Chunk.Width &&
            blockPosition.Y >= 0 && blockPosition.Y < Chunk.Height &&
            blockPosition.Z >= 0 && blockPosition.Z < Chunk.Width)
        {
            return _blocks[blockPosition.X, blockPosition.Y, blockPosition.Z];
        }

        return BlockType.Grass;
    }

    private void GenerateFrontFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[0];
        texCoords1[1] = block.TexCoords[1];
        texCoords1[2] = block.TexCoords[2];

        texCoords2[0] = block.TexCoords[3];
        texCoords2[1] = block.TexCoords[4];
        texCoords2[2] = block.TexCoords[5];

        vertcies1[0] = block.Vertices[0] + blockPosition;
        vertcies1[1] = block.Vertices[1] + blockPosition;
        vertcies1[2] = block.Vertices[2] + blockPosition;

        vertcies2[0] = block.Vertices[3] + blockPosition;
        vertcies2[1] = block.Vertices[4] + blockPosition;
        vertcies2[2] = block.Vertices[5] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[0]);
    }

    private void GenerateBackFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[6];
        texCoords1[1] = block.TexCoords[7];
        texCoords1[2] = block.TexCoords[8];

        texCoords2[0] = block.TexCoords[9];
        texCoords2[1] = block.TexCoords[10];
        texCoords2[2] = block.TexCoords[11];

        vertcies1[0] = block.Vertices[6] + blockPosition;
        vertcies1[1] = block.Vertices[7] + blockPosition;
        vertcies1[2] = block.Vertices[8] + blockPosition;

        vertcies2[0] = block.Vertices[9] + blockPosition;
        vertcies2[1] = block.Vertices[10] + blockPosition;
        vertcies2[2] = block.Vertices[11] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[1]);
    }

    private void GenerateTopFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[12];
        texCoords1[1] = block.TexCoords[13];
        texCoords1[2] = block.TexCoords[14];

        texCoords2[0] = block.TexCoords[15];
        texCoords2[1] = block.TexCoords[16];
        texCoords2[2] = block.TexCoords[17];

        vertcies1[0] = block.Vertices[12] + blockPosition;
        vertcies1[1] = block.Vertices[13] + blockPosition;
        vertcies1[2] = block.Vertices[14] + blockPosition;

        vertcies2[0] = block.Vertices[15] + blockPosition;
        vertcies2[1] = block.Vertices[16] + blockPosition;
        vertcies2[2] = block.Vertices[17] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[2]);
    }

    private void GenerateBottomFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[18];
        texCoords1[1] = block.TexCoords[19];
        texCoords1[2] = block.TexCoords[20];

        texCoords2[0] = block.TexCoords[21];
        texCoords2[1] = block.TexCoords[22];
        texCoords2[2] = block.TexCoords[23];

        vertcies1[0] = block.Vertices[18] + blockPosition;
        vertcies1[1] = block.Vertices[19] + blockPosition;
        vertcies1[2] = block.Vertices[20] + blockPosition;

        vertcies2[0] = block.Vertices[21] + blockPosition;
        vertcies2[1] = block.Vertices[22] + blockPosition;
        vertcies2[2] = block.Vertices[23] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[3]);
    }

    private void GenerateRightFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[24];
        texCoords1[1] = block.TexCoords[25];
        texCoords1[2] = block.TexCoords[26];

        texCoords2[0] = block.TexCoords[27];
        texCoords2[1] = block.TexCoords[28];
        texCoords2[2] = block.TexCoords[29];

        vertcies1[0] = block.Vertices[24] + blockPosition;
        vertcies1[1] = block.Vertices[25] + blockPosition;
        vertcies1[2] = block.Vertices[26] + blockPosition;

        vertcies2[0] = block.Vertices[27] + blockPosition;
        vertcies2[1] = block.Vertices[28] + blockPosition;
        vertcies2[2] = block.Vertices[29] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[4]);
    }

    private void GenerateLeftFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[30];
        texCoords1[1] = block.TexCoords[31];
        texCoords1[2] = block.TexCoords[32];

        texCoords2[0] = block.TexCoords[33];
        texCoords2[1] = block.TexCoords[34];
        texCoords2[2] = block.TexCoords[35];

        vertcies1[0] = block.Vertices[30] + blockPosition;
        vertcies1[1] = block.Vertices[31] + blockPosition;
        vertcies1[2] = block.Vertices[32] + blockPosition;

        vertcies2[0] = block.Vertices[33] + blockPosition;
        vertcies2[1] = block.Vertices[34] + blockPosition;
        vertcies2[2] = block.Vertices[35] + blockPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2, block.Normals[5]);
    }

    private void GenerateFace(Vector3[] vertices1, Vector3[] vertices2, Vector2[] texCoords1, Vector2[] texCoords2, Vector3 normal)
    { 
        var polygon1 = new Polygon(vertices1, texCoords1, normal);
        var polygon2 = new Polygon(vertices2, texCoords2, normal);

        _polygons.Add(polygon1);
        _polygons.Add(polygon2);
    } 
}
