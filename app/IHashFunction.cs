using System.Reflection;

namespace waterb.app;

public interface IHashFunction
{
    private static readonly Dictionary<Type, Dictionary<string, List<IHashFunction>>> _hashFunctions = new();
    
    static IHashFunction()
    {
        var hashFunctionTypes = AppDomain.CurrentDomain
            .GetAssemblies().Where(a => !a.IsDynamic)
            .SelectMany(TryGetTypesSafely)
            .Where(t => t is { IsValueType: true, IsEnum: false } && 
                typeof(IHashFunction<>).IsAssignableFrom(t));

        foreach (var hashFunctionType in hashFunctionTypes)
        {
            if (hashFunctionType != null)
            {
                var interfaceType = hashFunctionType.GetInterfaces().FirstOrDefault(
                    t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHashFunction<>));
                var processableType = interfaceType!.GetGenericArguments()[0];
                
                var hashFunction = (IHashFunction)Activator.CreateInstance(hashFunctionType)!;
                if (!_hashFunctions.TryGetValue(processableType, out var typedHashFunctions))
                {
                    _hashFunctions[processableType] = typedHashFunctions = 
                        new Dictionary<string, List<IHashFunction>>();
                }
                
                if (!typedHashFunctions.TryGetValue(hashFunction.HashName, out var hashFunctions))
                {
                    typedHashFunctions[hashFunction.HashName] = hashFunctions = [];
                }
                
                hashFunctions.Add(hashFunction);
            }
        }
    }

    private static IEnumerable<Type?> TryGetTypesSafely(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
    }
    
    
    public static bool TryGetHashFunctions<TProcessableType>(
        string name, out IReadOnlyList<IHashFunction>? functions)
    {
        functions = null;
        if (!_hashFunctions.TryGetValue(typeof(TProcessableType), out var hashFunctions))
        {
            return false;
        }

        if (!hashFunctions.TryGetValue(name, out var functionsRaw))
        {
            return false;
        }
        
        functions = functionsRaw;
        return true;
    }
    
    public string HashName { get; }
}

public interface IHashFunction<in TProcessableType> : IHashFunction
{
    public uint Process(TProcessableType value);
}