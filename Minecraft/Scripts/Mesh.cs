using OpenTK.Graphics.OpenGL;

public struct Mesh
{
    public int VAO { get; private set; }

    private int _vboVertex;
    private int _vboTexCoords;
    private int _vboNormals;

    public int PolygonCount { get; private set; }

    public Mesh(Polygon[] polygons)
    {
        SetPolygons(polygons);
    }

    public void SetPolygons(Polygon[] polygons)
    {
        PolygonCount = polygons.Length * 3;

        var data = PolygonParser.Parse(polygons);

        _vboNormals = CreateVBO(data.Normals, sizeof(float));
        _vboVertex = CreateVBO(data.Vertices, sizeof(float));
        _vboTexCoords = CreateVBO(data.TexCoords, sizeof(float));

        VAO = CreateVAO();
    }

    private int CreateVBO(List<float> data, int size)
    {
        var vbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Count * size, data.ToArray(), BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        return vbo;
    }

    private int CreateVAO()
    {
        var vao = GL.GenVertexArray();

        GL.BindVertexArray(vao);

        GL.EnableClientState(ArrayCap.VertexArray);
        GL.EnableClientState(ArrayCap.TextureCoordArray);
        GL.EnableClientState(ArrayCap.NormalArray);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboNormals);
        GL.NormalPointer(NormalPointerType.Float, 0, _vboNormals);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboTexCoords);
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboVertex);
        GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.DisableClientState(ArrayCap.VertexArray);
        GL.DisableClientState(ArrayCap.TextureCoordArray);
        GL.DisableClientState(ArrayCap.NormalArray);

        return vao;
    }
}
