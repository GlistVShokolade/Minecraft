using OpenTK.Graphics.OpenGL;

public static class TextureBinder
{
    public static void Bind(Texture texture)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture.ID);
    }

    public static void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
}
