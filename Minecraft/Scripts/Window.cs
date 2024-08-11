using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    private readonly Camera _camera;
    private readonly BlockContainer _blockContainer;

    private readonly ChunkMeshBuilder _meshBuilder;
    private readonly ChunkTerrainGenerator _terrainGenerator;

    private readonly FPSCounter _fpsCounter;
    private readonly FPSCounterView _fpsCounterView;

    private readonly Input _input;
    private readonly Time _time;
    
    private readonly LightSource _lightSource;
    private readonly World _world;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
        CursorState = CursorState.Grabbed;

        var terrainSettings = new NoiseSettings(0.14f, 0.5f, 10f, FastNoiseLite.NoiseType.Perlin, FastNoiseLite.FractalType.None, FastNoiseLite.RotationType3D.None);
        var caveSettings = new NoiseSettings(2f, 1f, 0.5f, FastNoiseLite.NoiseType.Perlin, FastNoiseLite.FractalType.DomainWarpProgressive, FastNoiseLite.RotationType3D.ImproveXZPlanes);

        _camera = new Camera(10f, 0.4f, Vector3.Zero, Vector3.One, Vector3.Zero);

        _meshBuilder = new ChunkMeshBuilder();
        _terrainGenerator = new ChunkTerrainGenerator(terrainSettings, caveSettings);
        _input = new Input(KeyboardState, MouseState);
        _time = new Time(this);

        _world = new World(_camera);
        _lightSource = new LightSource(new LightSettings(Vector4.Zero, Vector4.UnitX));
        _blockContainer = new BlockContainer();

        _fpsCounter = new FPSCounter();
        _fpsCounterView = new FPSCounterView(_fpsCounter, this);
    }

    protected override void OnLoad()
    {
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest);

        GL.CullFace(CullFaceMode.Back);

        var texture = TextureLoader.Load(@"Textures\Texture.png");

        TextureBinder.Bind(texture);

        GlobalUpdator.OnInit();     
            
        base.OnLoad();
    }

    protected override void OnUnload()
    {
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);

        TextureBinder.Unbind();

        GlobalUpdator.OnEnd();

        base.OnUnload();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        if (IsFocused == false)
        {
            return;
        }

        GL.ClearColor(Color4.LightBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GlobalRenderer.RenderAll();

        SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (IsFocused == false)
        {
            return;
        }

        GlobalUpdator.OnUpdate();

        if (KeyboardState.WasKeyDown(Keys.P))
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
        }
        else if (KeyboardState.WasKeyDown(Keys.O))
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
        }

        _camera.Move();
        _camera.Rotate();

        _fpsCounter.Calculate();

        base.OnUpdateFrame(args);
    }
}
