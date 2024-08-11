public static class GlobalUpdator
{
    private readonly static List<IInitable> _initables = new List<IInitable>();
    private readonly static List<IUpdatable> _updatables = new List<IUpdatable>();
    private readonly static List<IEndable> _endables = new List<IEndable>();


    public static void OnInit()
    {
        for (int i = 0; i < _initables.Count; i++)
        {
            _initables[i].Init();
        }

        _initables.Clear();
    }

    public static void OnEnd()
    {
        for (int i = 0; i < _endables.Count; i++)
        {
            _endables[i].End();
        }

        _endables.Clear();
    }

    public static void OnUpdate()
    {
        for (int i = 0; i < _updatables.Count; i++)
        {
            _updatables[i].Update();
        }
    }

    public static void RegisterInit(IInitable initable) => _initables.Add(initable);
    public static void RegisterUpdate(IUpdatable updatable) => _updatables.Add(updatable);
    public static void RegisterEnd(IEndable endable) => _endables.Add(endable);

    public static void UnregisterInit(IInitable initable) => _initables.Remove(initable);
    public static void UnregisterUpdate(IUpdatable updatable) => _updatables.Remove(updatable);
    public static void UnregisterEnd(IEndable endable) => _endables.Remove(endable);
}
