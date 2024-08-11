public struct ParsedPolygonsData
{
    public readonly List<float> Vertices;
    public readonly List<float> TexCoords;
    public readonly List<float> Normals;

    public ParsedPolygonsData(List<float> vertices, List<float> texCoords, List<float> normals)
    {
        Vertices = vertices;
        TexCoords = texCoords;
        Normals = normals;
    }
}
