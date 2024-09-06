using System.Collections.Concurrent;
using System.Reflection;

namespace Infra.Caching;

public class ReflectionCache
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesCache = new();

    public static PropertyInfo[] GetPropertiesByType(Type type)
    {
        if (PropertiesCache.TryGetValue(type, out var properties))
            return properties;
    
        properties = type.GetProperties();
        PropertiesCache.TryAdd(type, properties);
        return properties;
    }
}