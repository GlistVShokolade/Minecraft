public static class GlobalRenderer
{
    private readonly static List<Renderer> _renderers = new List<Renderer>();

    public static void RenderAll()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            _renderers[i].Render();
        }
    }

    public static void Register(Renderer renderer)
    {
        if (renderer == null)
        {
            throw new NullReferenceException(nameof(renderer));
        }
        if (_renderers.Contains(renderer))
        {
            return;
        }

        _renderers.Add(renderer);
    }

    public static void Unregister(Renderer renderer)
    {
        if (renderer == null)
        {
            throw new NullReferenceException(nameof(renderer));
        }
        if (_renderers.Contains(renderer) == false)
        {
            throw new KeyNotFoundException(nameof(renderer));
        }

        _renderers.Remove(renderer);
    }
}
