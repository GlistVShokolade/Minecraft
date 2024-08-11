using OpenTK.Graphics.OpenGL;

public class MeshRenderer : Renderer
{
    public Mesh Mesh { get; }

    public MeshRenderer(ref Mesh mesh)
    {
        Mesh = mesh;
    }

    public override void Render()
    {
        GL.BindVertexArray(Mesh.VAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.PolygonCount);
    }
}
