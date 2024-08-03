using OpenTK.Mathematics;

public class Grass : Block
{
    public Grass()
    {
        _blockType = BlockType.Grass;

        _texCoords = new Vector2[]
        {
            // front
            new Vector2(0.0625f, 0.9375f),
            new Vector2(0.0625f, 1f),
            new Vector2(0.125f, 0.9375f),

            new Vector2(0.125f, 0.9375f),
            new Vector2(0.0625f, 1f),
            new Vector2(0.125f, 1f),

            // back
            new Vector2(0.0625f, 0.9375f),
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.0625f, 1f),

            new Vector2(0.125f, 0.9375f),
            new Vector2(0.125f, 1f),
            new Vector2(0.0625f, 1f),

            // top
            new Vector2(0f, 0.9375f),
            new Vector2(0.0625f, 1f),
            new Vector2(0f, 1f),

            new Vector2(0.0625f, 1f),
            new Vector2(0f, 0.9375f),
            new Vector2(0f, 1f),

            // bottom
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.125f, 1f),

            new Vector2(0.1875f, 1f),
            new Vector2(0.125f, 1f),
            new Vector2(0.125f, 0.9375f),

            // right
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.125f, 1f),
            new Vector2(0.0625f, 0.9375f),

            new Vector2(0.125f, 1f),
            new Vector2(0.0625f, 1f),
            new Vector2(0.0625f, 0.9375f),

            // left
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.0625f, 0.9375f),
            new Vector2(0.125f, 1f),

            new Vector2(0.125f, 1f),
            new Vector2(0.0625f, 0.9375f),
            new Vector2(0.0625f, 1f),
        };

        _normals = new Vector3[]
        {
            NormalCalculator.Calculate(_vertices[0], _vertices[1], _vertices[2]),
            NormalCalculator.Calculate(_vertices[3], _vertices[4], _vertices[5]),
            NormalCalculator.Calculate(_vertices[6], _vertices[7], _vertices[8]),

            NormalCalculator.Calculate(_vertices[9], _vertices[10], _vertices[11]),
            NormalCalculator.Calculate(_vertices[12], _vertices[13], _vertices[14]),
            NormalCalculator.Calculate(_vertices[15], _vertices[16], _vertices[17]),

            NormalCalculator.Calculate(_vertices[18], _vertices[19], _vertices[20]),
            NormalCalculator.Calculate(_vertices[21], _vertices[22], _vertices[23]),
            NormalCalculator.Calculate(_vertices[24], _vertices[25], _vertices[26]),

            NormalCalculator.Calculate(_vertices[27], _vertices[28], _vertices[29]),
            NormalCalculator.Calculate(_vertices[30], _vertices[31], _vertices[32]),
            NormalCalculator.Calculate(_vertices[33], _vertices[34], _vertices[35]),
        };
    }
}

