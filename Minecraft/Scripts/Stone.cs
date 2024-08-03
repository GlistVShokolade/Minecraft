using OpenTK.Mathematics;

public class Stone : Block
{
    public Stone()
    {
        _texCoords = new Vector2[]
        {
            // front
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.1875f, 1f),
            new Vector2(0.250f, 0.9375f),

            new Vector2(0.250f, 0.9375f),
            new Vector2(0.1875f, 1f),
            new Vector2(0.250f, 1f),

            // back
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.250f, 0.9375f),
            new Vector2(0.1875f, 1f),

            new Vector2(0.250f, 0.9375f),
            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 1f),

            // top
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 1f),

            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.1875f, 1f),

            // bottom
            new Vector2(0.250f, 0.9375f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.250f, 1f),

            new Vector2(0.1875f, 1f),
            new Vector2(0.250f, 1f),
            new Vector2(0.250f, 0.9375f),

            // right
            new Vector2(0.250f, 0.9375f),
            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 0.9375f),

            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 1f),
            new Vector2(0.1875f, 0.9375f),

            // left
            new Vector2(0.250f, 0.9375f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.250f, 1f),

            new Vector2(0.250f, 1f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.1875f, 1f),
        };
    }
}