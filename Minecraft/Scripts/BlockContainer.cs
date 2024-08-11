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
