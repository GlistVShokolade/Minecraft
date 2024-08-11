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

    }
}

