using OpenTK.Mathematics;

public class Block
{
    protected BlockType _blockType;
    protected Vector2[] _texCoords;
    protected Vector3[] _vertices;
    protected Vector3[] _normals;

    public BlockType BlockType => _blockType;
    public Vector2[] TexCoords => _texCoords;
    public Vector3[] Vertices => _vertices;
    public Vector3[] Normals => _normals;

    public Block()
    {
        _vertices = new Vector3[]
        {
            // front
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(1f, 0f, 0f),

            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(1f, 1f, 0f),

            // back
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 1f, 1f),

            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            new Vector3(0f, 1f, 1f),

            // top
            new Vector3(0f, 1f, 1f),
            new Vector3(1f, 1f, 0f),
            new Vector3(0f, 1f, 0f),

            new Vector3(1f, 1f, 0f),
            new Vector3(0f, 1f, 1f),
            new Vector3(1f, 1f, 1f),

            // bottom
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 0f, 1f),

            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),

            //right
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 1f, 1f),
            new Vector3(0f, 0f, 0f),

            new Vector3(0f, 1f, 1f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 0f),

            //left
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 1f),

            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 0f),
        };
    }
}
