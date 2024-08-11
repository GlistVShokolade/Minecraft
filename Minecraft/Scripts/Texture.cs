public struct Texture
{
    public int ID { get; }

    public Texture(int textureId)
    {
        if (textureId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(textureId));
        }

        ID = textureId;
    }
}
