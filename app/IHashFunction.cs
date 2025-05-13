using System.Reflection;

namespace waterb.app;

public interface IHashFunction
{
    private static readonly List<Type> _rawGenericHashTypes = [];
    private static readonly Dictionary<string, List<Type>> _cachedBaseHashTypes = new();
    private static readonly Dictionary<Type, Dictionary<string, List<Type>>> _cachedGenericHashTypes = new();
    
    static IHashFunction()
    {
        var hashFunctionTypes = AppDomain.CurrentDomain
            .GetAssemblies().Where(a => !a.IsDynamic)
            .SelectMany(TryGetTypesSafely)
            .Where(t => t is { IsInterface: false, IsAbstract: false } &&
                typeof(IHashFunction).IsAssignableFrom(t)).Select(t => t!);

        foreach (var hashFunctionType in hashFunctionTypes)
        {
            var interfaceType = hashFunctionType.GetInterfaces().FirstOrDefault(
                t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHashFunction<>));
            if (interfaceType != null)
            {
                if (hashFunctionType.IsGenericType)
                {
                    _rawGenericHashTypes.Add(hashFunctionType);
                }
                else
                {
                    var baseHashFunction = (IHashFunction)Activator.CreateInstance(hashFunctionType)!;
                    if (!_cachedBaseHashTypes.TryGetValue(baseHashFunction.HashName, out var hashTypes))
                    {
                        _cachedBaseHashTypes[baseHashFunction.HashName] = hashTypes = [];
                    }
                    
                    hashTypes.Add(hashFunctionType);
                }
            }
        }
    }

    private static IEnumerable<Type?> TryGetTypesSafely(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
    }
    
    public static bool CreateHashFunctions<TProcessableType>(
        string name, out List<IHashFunction<TProcessableType>>? functions)
    {
        functions = null;
        if (!_cachedGenericHashTypes.TryGetValue(typeof(TProcessableType), out var namedHashFunctions))
        {
            functions = [];
            _cachedGenericHashTypes[typeof(TProcessableType)] = namedHashFunctions = [];
            for (var i = 0; i < _rawGenericHashTypes.Count; i++)
            {
                var hashFunctionType = _rawGenericHashTypes[i].MakeGenericType(typeof(TProcessableType));
                var baseHashFunction = (IHashFunction<TProcessableType>)Activator.CreateInstance(hashFunctionType)!;
                if (!namedHashFunctions.TryGetValue(baseHashFunction.HashName, out var hashTypes))
                {
                    namedHashFunctions[baseHashFunction.HashName] = hashTypes = [];
                }
                
                hashTypes.Add(hashFunctionType);
                functions.Add(baseHashFunction);
            }
            
            return functions.Count > 0;
        }
        
        if (!namedHashFunctions.TryGetValue(name, out var functionsTypes) || functionsTypes.Count == 0)
        {
            return false;
        }

        functions = new List<IHashFunction<TProcessableType>>(functionsTypes.Count);
        for (var i = 0; i < functionsTypes.Count; i++)
        {
            var baseHashFunction = (IHashFunction<TProcessableType>)Activator.CreateInstance(functionsTypes[i])!;
            functions.Add(baseHashFunction);
        }

        return true;
    }
    
    public string HashName { get; }
}

public interface IHashFunction<in TProcessableType> : IHashFunction
{
    public uint Process(TProcessableType value);
}