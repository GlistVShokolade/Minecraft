using OpenTK.Mathematics;

public struct Polygon
{
    public Vector3[] Vertices { get; }
    public Vector2[] TexCoords { get; } 
    public Vector3 Normal { get; }

    public Polygon(Vector3[] vertices, Vector2[] texCoords, Vector3 normal)
    {
        if (vertices.Length != 3)
        {
            throw new ArgumentOutOfRangeException(nameof(vertices.Length));
        }
        if (texCoords.Length != 3)
        {
            throw new ArgumentOutOfRangeException(nameof(texCoords.Length));
        }

        Vertices = vertices;
        TexCoords = texCoords;
        Normal = normal;
    }
}
