public abstract class Renderer
{
    public abstract void Render();

    public Renderer()
    {
        GlobalRenderer.Register(this);
    }

    ~Renderer()
    {
        GlobalRenderer.Unregister(this);
    }
}
