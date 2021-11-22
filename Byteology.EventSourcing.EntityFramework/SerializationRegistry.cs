namespace Byteology.EventSourcing.EntityFramework;

using System.Text.Json;

public class SerializationRegistry<TBaseType>
    where TBaseType : class
{
    private readonly JsonSerializerOptions _defaultSerializerOptions;
    private readonly Dictionary<Type, Func<TBaseType, string>> _serializationMap = new();
    private readonly Dictionary<string, Func<string, TBaseType>> _deserializationMap = new();

    public SerializationRegistry()
    {
        _defaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public SerializationRegistry(JsonSerializerOptions defaultSerializerOptions)
    {
        _defaultSerializerOptions = defaultSerializerOptions;
    }

    public void RegisterType<TType>()
        where TType : class, TBaseType
    {
        RegisterType<TType>(typeof(TType).Name);
    }

    public void RegisterType<TType>(string name)
        where TType : class, TBaseType
    {
        RegisterType(
            name,
            o => JsonSerializer.Serialize(o, _defaultSerializerOptions),
            s => JsonSerializer.Deserialize<TType>(s, _defaultSerializerOptions)!);
    }

    public void RegisterType<TType>(
        Func<TType, string> serializationMethod,
        Func<string, TType> deserializationMethod)
            where TType : class, TBaseType
    {
        RegisterType(typeof(TType).Name, serializationMethod, deserializationMethod);
    }

    public void RegisterType<TType>(
        string name, 
        Func<TType, string> serializationMethod, 
        Func<string, TType> deserializationMethod)
            where TType : class, TBaseType
    {
        if (_serializationMap.ContainsKey(typeof(TType)))
            throw new ArgumentException($"The type '{typeof(TType)}' is already registered.");

        if (_deserializationMap.ContainsKey(name))
            throw new ArgumentException($"The name '{name}' is already registed by another type.");


        _serializationMap.Add(typeof(TType), (o) => serializationMethod.Invoke((o as TType)!));
        _deserializationMap.Add(name, (s) => deserializationMethod.Invoke(s));
    }

    internal string Serialize(TBaseType arg)
    {
        Type type = arg.GetType();

        if (!_serializationMap.ContainsKey(type))
            throw new ArgumentException($"The type '{type}' is not registered.");

        string result = _serializationMap[type].Invoke(arg);
        return result;
    }

    internal TBaseType Deserialize(string name, string data)
    {
        if (!_deserializationMap.ContainsKey(name))
            throw new ArgumentException($"The name '{name}' is not registered.");

        TBaseType result = _deserializationMap[name].Invoke(data);
        return result;
    }
}
