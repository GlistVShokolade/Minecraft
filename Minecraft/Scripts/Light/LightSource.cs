using OpenTK.Graphics.OpenGL;

public class LightSource : IUpdatable
{
    private LightSettings _settings;

    public LightSource(LightSettings settings)
    {
        _settings = settings;

        GlobalUpdator.RegisterUpdate(this);
    }

    ~LightSource()
    {
        GlobalUpdator.UnregisterUpdate(this);
    }

    public void Light()
    {
        GL.Enable(EnableCap.Normalize);

        GL.PushMatrix();

        GL.Light(LightName.Light0, LightParameter.Position, _settings.Position);
        GL.Light(LightName.Light0, LightParameter.SpotDirection, _settings.Direction);

        GL.PopMatrix();

        GL.Disable(EnableCap.Normalize);
    }

    public void Update()
    {
        Light();
    }
}
