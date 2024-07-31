using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Drawing;

public class Program
{
    private static void Main(string[] args)
    {
        var gameSettings = new GameWindowSettings();
        var nativeSettings = new NativeWindowSettings();

        nativeSettings.Title = "Minecraft Like Game";
        nativeSettings.ClientSize = new Vector2i(1280, 720);
        nativeSettings.Vsync = VSyncMode.On;
        nativeSettings.Profile = ContextProfile.Compatability;

        using (var window = new Window(gameSettings, nativeSettings))
        {
            window.Run();
        }

        Console.WriteLine("Окно было создано");
    }
}

public class Window : GameWindow
{
    private readonly Camera _camera;

    private readonly ChunkMeshBuilder _meshBuilder;
    private readonly ChunkTerrainGenerator _terrainGenerator;
    private readonly ChunkSpawner _chunkSpawner;

    private readonly WorldUpdator _worldUpdator;
    private readonly WorldRenderer _worldRenderer;

    private readonly PolygonParser _polygonParser;
    private readonly FPSCounter _fpsCounter;

    private readonly World _world;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
        var terrainSettings = new NoiseSettings(0.14f, 0.5f, 10f, FastNoiseLite.NoiseType.Perlin, FastNoiseLite.FractalType.None, FastNoiseLite.RotationType3D.None);

        _camera = new Camera(10f, 0.4f, Vector3.Zero, Vector3.One, Quaternion.Identity);

        _meshBuilder = new ChunkMeshBuilder();
        _terrainGenerator = new ChunkTerrainGenerator(terrainSettings);
        _chunkSpawner = new ChunkSpawner();
        _worldUpdator = new WorldUpdator();
        _worldRenderer = new WorldRenderer();
        _polygonParser = new PolygonParser();
        _fpsCounter = new FPSCounter(this);
        _world = new World();
    }

    protected override void OnLoad()
    {
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);

        GL.CullFace(CullFaceMode.Back);

        base.OnLoad();
    }

    protected override void OnUnload()
    {
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);

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

        _worldRenderer.RenderAll();
        _fpsCounter.TryDraw();

        SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (IsFocused == false)
        {
            return;
        }

        if (KeyboardState.WasKeyDown(Keys.P))
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
        }
        else if (KeyboardState.WasKeyDown(Keys.O))
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
        }

        _camera.Move((float)args.Time, KeyboardState);
        _camera.Rotate(MouseState);

        _fpsCounter.Calculate(args.Time);

        base.OnUpdateFrame(args);
    }
}

public class FPSCounter
{
    private readonly GameWindow _window;

    private int _currentFPS;
    private double _frameTime;

    public FPSCounter(GameWindow window)
    {
        _window = window ?? throw new NullReferenceException(nameof(window));
    }

    public bool TryDraw()
    {
        if (_frameTime >= 1d)
        {
            Draw();

            return true;
        }

        return false;
    }

    public void Calculate(double frameTime)
    {
        if (_frameTime < 1d)
        {   
            _frameTime += frameTime;
            _currentFPS++;
        }
        else
        {
            _frameTime = 0d;
            _currentFPS = 0;
        }
    }

    private void Draw()
    {
        _window.Title = $"Minecraft Like Game | FPS: {_currentFPS}";
    }
}

public enum BlockType : byte
{
    Air,
    Grass,
    Stone,
}

public class WorldRenderer
{
    public static WorldRenderer Instance;

    private readonly List<Renderer> _renderers = new List<Renderer>();

    public WorldRenderer()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RenderAll()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            _renderers[i].Render();
        }
    }

    public void Register(Renderer renderer)
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

    public void Unregister(Renderer renderer)
    {
        if (renderer == null)
        {
            throw new NullReferenceException(nameof(renderer));
        }
        if (_renderers.Contains(renderer) == false)
        {
            throw new Exception("Скрипт рендера был не правально инициализирован!");
        }

        _renderers.Remove(renderer);
    }
}

public class WorldUpdator
{
    public static WorldUpdator Instance;

    private readonly List<IInitable> _initables = new List<IInitable>();
    private readonly List<IUpdatable> _updatables = new List<IUpdatable>();
    private readonly List<IEndable> _endables = new List<IEndable>();

    public WorldUpdator()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Register(IInitable initable) => _initables.Add(initable);
    public void Register(IUpdatable updatable) => _updatables.Add(updatable);
    public void Register(IEndable endable) => _endables.Add(endable);

    public void Unregister(IInitable initable) => _initables.Remove(initable);
    public void Unegister(IUpdatable updatable) => _updatables.Remove(updatable);
    public void Unregister(IEndable endable) => _endables.Remove(endable);
}

public interface IInitable
{
    public void Init();
}

public interface IUpdatable
{
    public void Update();
}

public interface IEndable
{
    public void End();
}

public class Constants
{
    public const int TickAmount = 20;
}

public class World
{
    public World()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                ChunkSpawner.Instance.Spawn(new Vector3i(x, 0, y) * Chunk.Width);
            }
        }
    }
}

public class Chunk : GameObject
{
    public const byte Width = 16;
    public const byte Height = 128;

    public Chunk(Transform transform, Renderer renderer) : base(transform, renderer)
    {
    }
}

public abstract class Renderer
{
    public abstract void Render();

    public Renderer()
    {
        WorldRenderer.Instance.Register(this);
    }

    ~Renderer()
    {
        WorldRenderer.Instance.Unregister(this);
    }
}

public class MeshRenderer : Renderer
{
    public Mesh Mesh { get; }

    public MeshRenderer(Mesh mesh)
    {
        Mesh = mesh ?? throw new NullReferenceException(nameof(mesh));
    }

    public override void Render()
    {
        GL.BindVertexArray(Mesh.VAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.PolygonCount);
    }
}

public class ChunkSpawner
{
    public static ChunkSpawner Instance;

    public ChunkSpawner()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Chunk Spawn(Vector3i worldPosition)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();

        var voxels = ChunkTerrainGenerator.Instance.Generate(worldPosition);
        var mesh = ChunkMeshBuilder.Instance.Generate(voxels, worldPosition);

        var transform = new Transform(worldPosition, Vector3.One, Quaternion.Identity);
        var renderer = new MeshRenderer(mesh);

        var chunk = new Chunk(transform, renderer);

        stopwatch.Stop();

        Console.WriteLine($"Time on generate chunk: {stopwatch.ElapsedMilliseconds}");

        return chunk;
    }
}

public class ChunkMeshBuilder
{
    public static ChunkMeshBuilder Instance;

    private List<Polygon> _polygons;

    private Vector3i _chunkPosition;
    private BlockType[,,] _blocks;

    public ChunkMeshBuilder()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Mesh Generate(BlockType[,,] blocks, Vector3i chunkPosition)
    {
        _polygons = new List<Polygon>(blocks.Length * 12);

        _blocks = blocks;
        _chunkPosition = chunkPosition;

        Console.WriteLine(blocks.Length);

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int z = 0; z < Chunk.Width; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        return new Mesh(_polygons.ToArray());
    }

    private void GenerateBlock(int x, int y, int z)
    {
        if (_blocks[x, y, z] == BlockType.Air)
        {
            return;
        }

        var blockPosition = new Vector3i(x, y, z);

        if (GetBlock(blockPosition + Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateBackFace(blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateFrontFace(blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitX) == BlockType.Air)
        {
            GenerateRightFace(blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitX) == BlockType.Air)
        {
            GenerateLeftFace(blockPosition + _chunkPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitY) == BlockType.Air)
        {
            GenerateTopFace(blockPosition + _chunkPosition);
        }
        if (blockPosition.Y > 0 && GetBlock(blockPosition - Vector3i.UnitY) == BlockType.Air)
        {
            GenerateBottomFace(blockPosition + _chunkPosition);
        }
    }

    private BlockType GetBlock(Vector3i blockPosition)
    {
        if (blockPosition.X >= 0 && blockPosition.X < Chunk.Width &&
            blockPosition.Y >= 0 && blockPosition.Y < Chunk.Height &&
            blockPosition.Z >= 0 && blockPosition.Z < Chunk.Width)
        {
            return _blocks[blockPosition.X, blockPosition.Y, blockPosition.Z];
        }

        return BlockType.Grass;
    }

    private void GenerateFrontFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(0f, 0f, 0f) + blockPosition;
        vertcies1[2] = new Vector3(1f, 0f, 0f) + blockPosition;
        vertcies1[1] = new Vector3(0f, 1f, 0f) + blockPosition;

        vertcies2[0] = new Vector3(1f, 0f, 0f) + blockPosition;
        vertcies2[2] = new Vector3(1f, 1f, 0f) + blockPosition;
        vertcies2[1] = new Vector3(0f, 1f, 0f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateBackFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(0f, 0f, 1f) + blockPosition;
        vertcies1[1] = new Vector3(1f, 0f, 1f) + blockPosition;
        vertcies1[2] = new Vector3(0f, 1f, 1f) + blockPosition;

        vertcies2[0] = new Vector3(1f, 0f, 1f) + blockPosition;
        vertcies2[1] = new Vector3(1f, 1f, 1f) + blockPosition;
        vertcies2[2] = new Vector3(0f, 1f, 1f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateTopFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(0f, 1f, 0f) + blockPosition;
        vertcies1[1] = new Vector3(0f, 1f, 1f) + blockPosition;
        vertcies1[2] = new Vector3(1f, 1f, 0f) + blockPosition;

        vertcies2[0] = new Vector3(0f, 1f, 1f) + blockPosition;
        vertcies2[1] = new Vector3(1f, 1f, 1f) + blockPosition;
        vertcies2[2] = new Vector3(1f, 1f, 0f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateBottomFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(0f, 0f, 0f) + blockPosition;
        vertcies1[2] = new Vector3(0f, 0f, 1f) + blockPosition;
        vertcies1[1] = new Vector3(1f, 0f, 0f) + blockPosition;

        vertcies2[0] = new Vector3(0f, 0f, 1f) + blockPosition;
        vertcies2[2] = new Vector3(1f, 0f, 1f) + blockPosition;
        vertcies2[1] = new Vector3(1f, 0f, 0f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateRightFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(0f, 0f, 1f) + blockPosition;
        vertcies1[2] = new Vector3(0f, 0f, 0f) + blockPosition;
        vertcies1[1] = new Vector3(0f, 1f, 1f) + blockPosition;

        vertcies2[0] = new Vector3(0f, 1f, 1f) + blockPosition;
        vertcies2[2] = new Vector3(0f, 0f, 0f) + blockPosition;
        vertcies2[1] = new Vector3(0f, 1f, 0f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateLeftFace(Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        vertcies1[0] = new Vector3(1f, 0f, 1f) + blockPosition;
        vertcies1[1] = new Vector3(1f, 0f, 0f) + blockPosition;
        vertcies1[2] = new Vector3(1f, 1f, 1f) + blockPosition;

        vertcies2[0] = new Vector3(1f, 1f, 1f) + blockPosition;
        vertcies2[1] = new Vector3(1f, 0f, 0f) + blockPosition;
        vertcies2[2] = new Vector3(1f, 1f, 0f) + blockPosition;

        GenerateFace(vertcies1, vertcies2);
    }

    private void GenerateFace(Vector3[] vertices1, Vector3[] vertices2)
    {
        var polygon1 = new Polygon(vertices1, Color4.DarkGreen);
        var polygon2 = new Polygon(vertices2, Color4.DarkGreen);

        _polygons.Add(polygon1);
        _polygons.Add(polygon2);
    }
}

public struct NoiseSettings
{
    public float Amplitude { get; }
    public float Frequency { get; }
    public float Depth { get; }
    public FastNoiseLite.NoiseType NoiseType { get; }
    public FastNoiseLite.FractalType FractalType { get; }
    public FastNoiseLite.RotationType3D RotationType { get; }

    public NoiseSettings(float amplitude, float frequency, float depth, FastNoiseLite.NoiseType noiseType, FastNoiseLite.FractalType fractalType, FastNoiseLite.RotationType3D rotationType)
    {
        Depth = depth;
        Amplitude = amplitude;
        Frequency = frequency;
        NoiseType = noiseType;
        FractalType = fractalType;
        RotationType = rotationType;
    }
}

public class ChunkTerrainGenerator
{
    public static ChunkTerrainGenerator Instance;

    private FastNoiseLite _noise;

    private readonly NoiseSettings _settings;

    public ChunkTerrainGenerator(NoiseSettings settings)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _noise = new FastNoiseLite();

        _noise.SetNoiseType(settings.NoiseType);
        _noise.SetFrequency(settings.Frequency);
        _noise.SetFractalType(settings.FractalType);
        _noise.SetRotationType3D(settings.RotationType);

        _settings = settings;
    }

    public BlockType[,,] Generate(Vector3i chunkPosition)
    {
        var blocks = new BlockType[Chunk.Width, Chunk.Height, Chunk.Width];

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int z = 0; z < Chunk.Width; z++)
            {
                float height = _noise.GetNoise(
                    (x + chunkPosition.X) * _settings.Amplitude,
                    (z + chunkPosition.Z) * _settings.Amplitude) * _settings.Depth + 32;

                for (int y = 0; y < height; y++)
                {
                    blocks[x, y, z] = BlockType.Grass;
                }
            }
        }

        return blocks;
    }
}

public abstract class Factory<Config, GameObject>
{
    public abstract GameObject Create(Config config);
    public abstract void Delete(GameObject gameObject);
}

public class Mesh
{
    public Polygon[] Polygons { get; }
    public int VAO { get; }

    private readonly int _vboVertices;
    private readonly int _vboColors;

    public int PolygonCount => Polygons.Length * 3;

    public Mesh(Polygon[] polygons)
    {
        Polygons = polygons;

        var data = PolygonParser.Instance.Parse(polygons);

        _vboVertices = CreateVBO(data.Vertices, sizeof(float));
        _vboColors = CreateVBO(data.Colors, sizeof(float));

        VAO = CreateVAO();
    }

    private int CreateVBO(float[] data, int size)
    {
        var vbo = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * size, data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        return vbo;
    }

    private int CreateVAO()
    {
        var vao = GL.GenVertexArray();

        GL.BindVertexArray(vao);

        GL.EnableClientState(ArrayCap.VertexArray);
        GL.EnableClientState(ArrayCap.ColorArray);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboVertices);
        GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboColors);
        GL.ColorPointer(4, ColorPointerType.Float, 0, 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.DisableClientState(ArrayCap.VertexArray);
        GL.DisableClientState(ArrayCap.ColorArray);

        return vao;
    }
}

public struct ParsedPolygonsData
{
    public float[] Vertices { get; }
    public float[] Colors { get; }

    public ParsedPolygonsData(float[] vertices, float[] colors)
    {
        Vertices = vertices;
        Colors = colors;
    }
}

public class PolygonParser
{
    public static PolygonParser Instance;

    public PolygonParser()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public ParsedPolygonsData Parse(Polygon[] polygons)
    {
        var vertices = new List<float>();
        var colors = new List<float>();

        for (int i = 0; i < polygons.Length; i++)
        {
            for (int j = 0; j < polygons[i].Vertices.Length; j++)
            {
                vertices.Add(polygons[i].Vertices[j].X);
                vertices.Add(polygons[i].Vertices[j].Y);
                vertices.Add(polygons[i].Vertices[j].Z);

                colors.Add(polygons[i].Color.R);
                colors.Add(polygons[i].Color.G);
                colors.Add(polygons[i].Color.B);
                colors.Add(polygons[i].Color.A);
            }
        }

        return new ParsedPolygonsData(vertices.ToArray(), colors.ToArray());
    }
}

public struct Polygon
{
    public Vector3[] Vertices { get; }
    public Color4 Color { get; }

    public Polygon(Vector3[] vertices, Color4 color)
    {
        if (vertices.Length != 3)
        {
            throw new ArgumentOutOfRangeException(nameof(vertices));
        }

        Vertices = vertices;
        Color = color;
    }
}
