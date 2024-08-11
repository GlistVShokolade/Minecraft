using OpenTK.Graphics.OpenGL;
using StbImageSharp;

public static class TextureLoader
{
    public static Texture Load(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);

        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

        var textureID = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 4);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return new Texture(textureID);
    }
}
