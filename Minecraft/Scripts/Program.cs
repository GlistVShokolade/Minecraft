using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

public class Program
{
    private static void Main(string[] args)
    {
        var gameSettings = new GameWindowSettings();
        var nativeSettings = new NativeWindowSettings();

        nativeSettings.ClientSize = new Vector2i(1280, 720);
        nativeSettings.Vsync = VSyncMode.On;
        nativeSettings.Profile = ContextProfile.Compatability;

        using (var window = new Window(gameSettings, nativeSettings))
        {
            window.Run();
        }
    }
}

public class Window : GameWindow
{
    private readonly Camera _camera;
    private readonly BlockContainer _blockContainer;

    private readonly ChunkMeshBuilder _meshBuilder;
    private readonly ChunkTerrainGenerator _terrainGenerator;

    private readonly WorldUpdator _worldUpdator;
    private readonly WorldRenderer _worldRenderer;

    private readonly FPSCounter _fpsCounter;

    private readonly Input _input;
    private readonly Time _time;
    
    private readonly World _world;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
        CursorState = CursorState.Grabbed;

        var terrainSettings = new NoiseSettings(0.14f, 0.5f, 10f, FastNoiseLite.NoiseType.Perlin, FastNoiseLite.FractalType.None, FastNoiseLite.RotationType3D.None);
        var caveSettings = new NoiseSettings(2f, 1f, 0.5f, FastNoiseLite.NoiseType.Perlin, FastNoiseLite.FractalType.DomainWarpProgressive, FastNoiseLite.RotationType3D.ImproveXZPlanes);

        _camera = new Camera(10f, 0.4f, Vector3.Zero, Vector3.One, Quaternion.Identity);

        _meshBuilder = new ChunkMeshBuilder();
        _terrainGenerator = new ChunkTerrainGenerator(terrainSettings, caveSettings);
        _worldUpdator = new WorldUpdator();
        _worldRenderer = new WorldRenderer();
        _fpsCounter = new FPSCounter(this);
        _input = new Input(KeyboardState, MouseState);
        _time = new Time(this);

        _world = new World(_camera);
        _blockContainer = new BlockContainer();
    }

    protected override void OnLoad()
    {
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Texture2D);

        GL.CullFace(CullFaceMode.Back);

        var texture = TextureHandler.Load(@"Textures\Texture.png");

        TextureHandler.Bind(texture);

        _worldUpdator.OnInit();
            
        base.OnLoad();
    }

    protected override void OnUnload()
    {
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Texture2D);

        TextureHandler.Unbind();

        _worldUpdator.OnEnd();

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

        _worldUpdator.OnUpdate();

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

public class LightSource : IUpdatable
{
    private LightSettings _settings;

    public LightSource(LightSettings settings)
    {
        _settings = settings;

        WorldUpdator.Instance.RegisterUpdate(this);
    }

    ~LightSource()
    {
        WorldUpdator.Instance.UnregisterUpdate(this);
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

public struct LightSettings
{
    public Vector4 Position;
    public Vector4 Direction;
    public EnableCap LightID;
    public LightName LightName;

    public LightSettings(Vector4 position, Vector4 direction, EnableCap lightID, LightName lightName)
    {
        Position = position;
        Direction = direction;
        LightID = lightID;
        LightName = lightName;
    }
}

public class Input
{
    public static Input Instance;

    private readonly KeyboardState _keyboard;
    private readonly MouseState _mouse;

    public Vector2 MousePosition => _mouse.Position;

    public Input(KeyboardState keyboard, MouseState mouse)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _keyboard = keyboard ?? throw new NullReferenceException(nameof(keyboard));
        _mouse = mouse ?? throw new NullReferenceException(nameof(mouse));
    }

    public bool IsKeyPressed(Keys key)
    {
        return _keyboard.IsKeyPressed(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return _keyboard.IsKeyReleased(key);
    }
    public bool IsKeyDown(Keys key)
    {
        return _keyboard.IsKeyDown(key);
    }
}

public class FPSCounter
{
    private readonly GameWindow _window;

    private int _currentFPS;
    private float _frameTime;

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

    public void Calculate()
    {
        if (_frameTime < 1d)
        {   
            _frameTime += Time.Instance.UpdateTime;
            _currentFPS++;
        }
        else
        {
            _frameTime = 0f;
            _currentFPS = 0;
        }
    }

    private void Draw()
    {
        _window.Title = $"Minecraft Like Game | FPS: { _currentFPS}";
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

    public void OnInit()
    {
        for (int i = 0; i < _initables.Count; i++)
        {
            _initables[i].Init();
        }

        _initables.Clear();
    }

    public void OnEnd()
    {
        for (int i = 0; i < _endables.Count; i++)
        {
            _endables[i].End();
        }

        _endables.Clear();
    }

    public void OnUpdate()
    {
        for (int i = 0; i < _updatables.Count; i++)
        {
            _updatables[i].Update();
        }
    }

    public void RegisterInit(IInitable initable) => _initables.Add(initable);
    public void RegisterUpdate(IUpdatable updatable) => _updatables.Add(updatable);
    public void RegisterEnd(IEndable endable) => _endables.Add(endable);

    public void UnregisterInit(IInitable initable) => _initables.Remove(initable);
    public void UnregisterUpdate(IUpdatable updatable) => _updatables.Remove(updatable);
    public void UnregisterEnd(IEndable endable) => _endables.Remove(endable);
}

public class Time
{
    public static Time Instance;

    private readonly GameWindow _window;

    public float UpdateTime => (float)_window.UpdateTime;

    public Time(GameWindow window)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _window = window ?? throw new NullReferenceException(nameof(window));
    }
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
    private readonly ChunkSpawner _spawner;
    private readonly ChunkLoader _loader;

    public World(Transform player)
    {
        _loader = new ChunkLoader(player);
        _spawner = new ChunkSpawner(_loader);
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
        Mesh = mesh;
    }

    public override void Render()
    {
        GL.BindVertexArray(Mesh.VAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.PolygonCount);
    }
}

public class ChunkLoader : IUpdatable
{
    private readonly Transform _player;

    private Vector2i _lastChunkPosition = Vector2i.UnitY;
    private Vector2i PlayerChunkPosition => new Vector2i((int)_player.Position.X, (int)_player.Position.Z) / Chunk.Width;

    private readonly Dictionary<Vector2i, LoadedChunkData> _loadedChunks = new Dictionary<Vector2i, LoadedChunkData>();

    public event Action<LoadedChunkData[]> ChunksLoaded;

    public ChunkLoader(Transform player)
    {
        _player = player ?? throw new NullReferenceException(nameof(player));

        WorldUpdator.Instance.RegisterUpdate(this);
    }

    ~ChunkLoader()
    {
        WorldUpdator.Instance.UnregisterUpdate(this);
    }


    public void LoadChunks()
    {
        var currentChunkPosition = PlayerChunkPosition;

        for (int x = currentChunkPosition.X - Camera.ViewRadius; x < currentChunkPosition.X + Camera.ViewRadius; x++)
        {
            for (int y = currentChunkPosition.Y - Camera.ViewRadius; y < currentChunkPosition.Y + Camera.ViewRadius; y++)
            {
                var localPosition = new Vector2i(x, y); 

                if (_loadedChunks.ContainsKey(localPosition))
                {
                    continue;
                }

                var distance = Vector2.Distance(currentChunkPosition, localPosition);
                
                if (distance > Camera.ViewRadius)
                {
                    continue;
                }

                var worldPosition = new Vector3i(x, 0, y) * Chunk.Width;

               LoadChunk(worldPosition, localPosition);
            }
        }

        ChunksLoaded?.Invoke(_loadedChunks.Values.ToArray());
    }

    private void LoadChunk(Vector3i worldPosition, Vector2i localPosition)
    {
        var loadedChunkData = new LoadedChunkData();

        var blocks = ChunkTerrainGenerator.Instance.Generate(worldPosition);
        var mesh = ChunkMeshBuilder.Instance.Generate(blocks, worldPosition);

        loadedChunkData.Mesh = mesh;
        loadedChunkData.WorldPosition = worldPosition;

        _loadedChunks.Add(localPosition, loadedChunkData);
    }

    public void Update()
    {
        TryLoad();
    }

    private bool TryLoad()
    {
        var currentChunkPosition = PlayerChunkPosition;

        if (_lastChunkPosition != currentChunkPosition)
        {
            _lastChunkPosition = currentChunkPosition;

            LoadChunks();

            return true;
        }

        return false;
    }
}

public static class TextureHandler
{
    public static Texture Load(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlue);

        var id = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, id);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 4);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return new Texture(id);
    }

    public static void Bind(Texture texture)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture.ID);
    }

    public static void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
}

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

public class ChunkSpawner
{
    private readonly ChunkLoader _chunkLoader;
    
    public ChunkSpawner(ChunkLoader chunkLoader)
    {
        _chunkLoader = chunkLoader ?? throw new NullReferenceException(nameof(chunkLoader));

        _chunkLoader.ChunksLoaded += SpawnChunks;
    }

    ~ChunkSpawner()
    {
        _chunkLoader.ChunksLoaded -= SpawnChunks;
    }

    private void SpawnChunks(LoadedChunkData[] loadedChunkDatas)
    {
        for (int i = 0; i < loadedChunkDatas.Length; i++)
        {
            SpawnChunk(loadedChunkDatas[i]);
        }
    }

    private void SpawnChunk(LoadedChunkData loadedChunkData)
    {
        var transform = new Transform(loadedChunkData.WorldPosition, Vector3.One, Quaternion.Identity);
        var renderer = new MeshRenderer(loadedChunkData.Mesh);

        var chunk = new Chunk(transform, renderer);
    }
}

public enum ChunkLoadState : byte
{
    Start,
    Loaded,
    Complete,
}

public class BlockContainer
{
    private static Dictionary<BlockType, Block> _blocks = new Dictionary<BlockType, Block>();
    
    public BlockContainer()
    {
        _blocks.Add(BlockType.Grass, new Grass());
        _blocks.Add(BlockType.Stone, new Stone());
    }

    public static Block Get(BlockType type)
    {
        return _blocks[type];
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
        _polygons = new List<Polygon>();

        _blocks = blocks;
        _chunkPosition = chunkPosition;

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

        var polygons = _polygons.ToArray();

        _blocks = null;
        _polygons = null;

        return new Mesh(polygons);
    }

    private void GenerateBlock(int x, int y, int z)
    {
        if (_blocks[x, y, z] == BlockType.Air)
        {
            return;
        }

        var block = BlockContainer.Get(_blocks[x, y, z]);
        var blockPosition = new Vector3i(x, y, z);

        if (GetBlock(blockPosition + Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateBackFace(block, blockPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitZ) == BlockType.Air)
        {
            GenerateFrontFace(block, blockPosition);
        }
        if (GetBlock(blockPosition - Vector3i.UnitX) == BlockType.Air)
        {
            GenerateRightFace(block, blockPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitX) == BlockType.Air)
        {
            GenerateLeftFace(block, blockPosition);
        }
        if (GetBlock(blockPosition + Vector3i.UnitY) == BlockType.Air)
        {
            GenerateTopFace(block, blockPosition);
        }
        if (blockPosition.Y > 0 && GetBlock(blockPosition - Vector3i.UnitY) == BlockType.Air)
        {
            GenerateBottomFace(block, blockPosition);
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

    private void GenerateFrontFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[0];
        texCoords1[1] = block.TexCoords[1];
        texCoords1[2] = block.TexCoords[2];

        texCoords2[0] = block.TexCoords[3];
        texCoords2[1] = block.TexCoords[4];
        texCoords2[2] = block.TexCoords[5];

        vertcies1[0] = block.Vertices[0] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[1] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[2] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[3] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[4] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[5] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateBackFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[6];
        texCoords1[1] = block.TexCoords[7];
        texCoords1[2] = block.TexCoords[8];

        texCoords2[0] = block.TexCoords[9];
        texCoords2[1] = block.TexCoords[10];
        texCoords2[2] = block.TexCoords[11];

        vertcies1[0] = block.Vertices[6] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[7] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[8] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[9] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[10] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[11] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateTopFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[12];
        texCoords1[1] = block.TexCoords[13];
        texCoords1[2] = block.TexCoords[14];

        texCoords2[0] = block.TexCoords[15];
        texCoords2[1] = block.TexCoords[16];
        texCoords2[2] = block.TexCoords[17];

        vertcies1[0] = block.Vertices[12] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[13] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[14] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[15] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[16] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[17] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateBottomFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[18];
        texCoords1[1] = block.TexCoords[19];
        texCoords1[2] = block.TexCoords[20];

        texCoords2[0] = block.TexCoords[21];
        texCoords2[1] = block.TexCoords[22];
        texCoords2[2] = block.TexCoords[23];

        vertcies1[0] = block.Vertices[18] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[19] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[20] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[21] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[22] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[23] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateRightFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[24];
        texCoords1[1] = block.TexCoords[25];
        texCoords1[2] = block.TexCoords[26];

        texCoords2[0] = block.TexCoords[27];
        texCoords2[1] = block.TexCoords[28];
        texCoords2[2] = block.TexCoords[29];

        vertcies1[0] = block.Vertices[24] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[25] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[26] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[27] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[28] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[29] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateLeftFace(Block block, Vector3i blockPosition)
    {
        var vertcies1 = new Vector3[3];
        var vertcies2 = new Vector3[3];

        var texCoords1 = new Vector2[3];
        var texCoords2 = new Vector2[3];

        texCoords1[0] = block.TexCoords[30];
        texCoords1[1] = block.TexCoords[31];
        texCoords1[2] = block.TexCoords[32];

        texCoords2[0] = block.TexCoords[33];
        texCoords2[1] = block.TexCoords[34];
        texCoords2[2] = block.TexCoords[35];

        vertcies1[0] = block.Vertices[30] + blockPosition + _chunkPosition;
        vertcies1[1] = block.Vertices[31] + blockPosition + _chunkPosition;
        vertcies1[2] = block.Vertices[32] + blockPosition + _chunkPosition;

        vertcies2[0] = block.Vertices[33] + blockPosition + _chunkPosition;
        vertcies2[1] = block.Vertices[34] + blockPosition + _chunkPosition;
        vertcies2[2] = block.Vertices[35] + blockPosition + _chunkPosition;

        GenerateFace(vertcies1, vertcies2, texCoords1, texCoords2);
    }

    private void GenerateFace(Vector3[] vertices1, Vector3[] vertices2, Vector2[] texCoords1, Vector2[] texCoords2)
    { 
        var polygon1 = new Polygon(vertices1, texCoords1);
        var polygon2 = new Polygon(vertices2, texCoords2);

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

    private FastNoiseLite _terrainNoise;
    private FastNoiseLite _caveNoise;

    private readonly NoiseSettings _terrainSettings;
    private readonly NoiseSettings _caveSettings;

    public ChunkTerrainGenerator(NoiseSettings terrainSettings, NoiseSettings caveSettings)
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetNoise(terrainSettings, ref _terrainNoise);
        SetNoise(caveSettings, ref _caveNoise);

        _terrainSettings = terrainSettings;
        _caveSettings = terrainSettings;
    }

    private void SetNoise(NoiseSettings settings, ref FastNoiseLite noise)
    {
        noise = new FastNoiseLite();

        noise.SetNoiseType(settings.NoiseType);
        noise.SetFrequency(settings.Frequency);
        noise.SetFractalType(settings.FractalType);
        noise.SetRotationType3D(settings.RotationType);
    }

    public BlockType[,,] Generate(Vector3i chunkPosition)
    {
        var blocks = new BlockType[Chunk.Width, Chunk.Height, Chunk.Width];

        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int z = 0; z < Chunk.Width; z++)
            {
                float terrainHeight = _terrainNoise.GetNoise(
                    (x + chunkPosition.X) * _terrainSettings.Amplitude,
                    (z + chunkPosition.Z) * _terrainSettings.Amplitude) * _terrainSettings.Depth + 32;

                for (int y = 0; y < terrainHeight; y++)
                {
                    if (y == (int)terrainHeight)
                    {
                        blocks[x, y, z] = BlockType.Grass;
                    }
                    else if ((int)terrainHeight - y < (int)terrainHeight)
                    {
                        blocks[x, y, z] = BlockType.Stone;
                    }
                }

                for (int y = 0; y < Chunk.Height; y++)
                {
                    float caveWeight = _caveNoise.GetNoise((x + chunkPosition.X) * _caveSettings.Amplitude, y * _caveSettings.Amplitude, (z + chunkPosition.Z) * _caveSettings.Amplitude) * _caveSettings.Depth;

                    if (caveWeight >= 1.5f)
                    {
                        blocks[x, y, z] = BlockType.Air;
                    }
                }
            }
        }

        return blocks;
    }
}

public struct Mesh
{
    public int VAO { get; private set; }

    private int _vboVertices;
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
        _vboVertices = CreateVBO(data.Vertices, sizeof(float));
        _vboTexCoords = CreateVBO(data.TexCoords, sizeof(float));

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
        GL.EnableClientState(ArrayCap.TextureCoordArray);
        GL.EnableClientState(ArrayCap.NormalArray);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboNormals);
        GL.NormalPointer(NormalPointerType.Float, 0, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboVertices);
        GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboTexCoords);
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.DisableClientState(ArrayCap.VertexArray);
        GL.DisableClientState(ArrayCap.TextureCoordArray);
        GL.DisableClientState(ArrayCap.NormalArray);

        return vao;
    }
}

public static class Timer
{
    public static void Wait(float time)
    {
        while (time > 0f)
        {
            time -= Time.Instance.UpdateTime;
        }
    }
}

public struct ParsedPolygonsData
{
    public readonly float[] Vertices;
    public readonly float[] TexCoords;
    public readonly float[] Normals;

    public ParsedPolygonsData(float[] vertices, float[] texCoords, float[] normals)
    {
        Vertices = vertices;
        TexCoords = texCoords;
        Normals = normals;
    }
}

public static class PolygonParser
{
    public static ParsedPolygonsData Parse(Polygon[] polygons)
    {
        var vertices = new List<float>();
        var texcoords = new List<float>();
        var normals = new List<float>();

        for (int i = 0; i < polygons.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vertices.Add(polygons[i].Vertices[j].X);
                vertices.Add(polygons[i].Vertices[j].Y);
                vertices.Add(polygons[i].Vertices[j].Z);

                texcoords.Add(polygons[i].TexCoords[j].X);
                texcoords.Add(polygons[i].TexCoords[j].Y);
            }

            normals.Add(polygons[i].Normal.X);
            normals.Add(polygons[i].Normal.Y);
            normals.Add(polygons[i].Normal.Z);
        }

        return new ParsedPolygonsData(vertices.ToArray(), texcoords.ToArray(), normals.ToArray());
    }
}

public static class NormalCalculator
{
    public readonly static Vector3 LightDirection = new Vector3(0f, 0.5f, 1f);

    public static Vector3 Calculate(Vector3 a, Vector3 b, Vector3 c)
    {
        return (c - a) * (b - a);
    }
}

public struct Polygon
{
    public Vector3[] Vertices { get; }
    public Vector2[] TexCoords { get; } 
    public Vector3 Normal { get; }

    public Polygon(Vector3[] vertices, Vector2[] texCoords)
    {
        if (vertices.Length != 3)
        {
            throw new ArgumentOutOfRangeException(nameof(vertices.Length));
        }
        if (texCoords.Length != 3)
        {
            throw new ArgumentOutOfRangeException(nameof(texCoords.Length));
        }

        Normal = NormalCalculator.Calculate(vertices[0], vertices[1], vertices[2]);
        Vertices = vertices;
        TexCoords = texCoords;
    }
}
