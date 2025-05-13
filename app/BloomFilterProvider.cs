namespace waterb.app;

public sealed class BloomFilterProvider<T> : IWebServerService
{
    private readonly Dictionary<string, BloomFilter<T>> _bloomFilters = new();
    
    public void Add(string name, BloomFilter<T> filter) => _bloomFilters.Add(name, filter);
    public bool TryAdd(string name, BloomFilter<T> filter) => _bloomFilters.TryAdd(name, filter);
    public void Set(string name, BloomFilter<T> filter) => _bloomFilters[name] = filter;
    public bool TryRemove(string name, out BloomFilter<T>? filter) => _bloomFilters.Remove(name, out filter);
    public void Clear() => _bloomFilters.Clear();
    
    public BloomFilter<T> Get(string name) => _bloomFilters[name];
    public bool TryGet(string name, out BloomFilter<T>? filter) => _bloomFilters.TryGetValue(name, out filter);
}