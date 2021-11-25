namespace Byteology.EventSourcing.Configuration;

using System.Reflection;

public class TypeRegistry<TBaseType>
{
    private readonly Dictionary<Type, string> _typeToString = new();
    private readonly Dictionary<string, Type> _stringToType = new();

    public void RegisterType(Type type)
    {
        if (!type.IsAssignableTo(typeof(TBaseType)))
            throw new ArgumentException($"The specified type can't be assigned to {typeof(TBaseType)}.");

        if (_typeToString.ContainsKey(type))
            return;

        string name = getNameOfType(type);

        if (_stringToType.ContainsKey(name))
            throw new InvalidOperationException($"Another type with the same name was already registered.");

        _typeToString.Add(type, name);
        _stringToType.Add(name, type);
    }
    public void RegisterType<TType>()
        where TType : TBaseType => RegisterType(typeof(TType));

    public void RegisterTypes(IEnumerable<Type> types)
    {
        foreach (Type type in types)
            RegisterType(type);
    }

    public void RegisterTypesInAssembly(Assembly assembly)
    {
        IEnumerable<Type> allTypes = assembly.GetTypes();
        foreach (Type type in allTypes)
            if (type.IsAssignableTo(typeof(TBaseType)))
                RegisterType(type);
    }
    public void RegisterTypesInAssemblies(IEnumerable<Assembly> assemblies)
    {
        foreach (Assembly assembly in assemblies)
            RegisterTypesInAssembly(assembly);
    }

    public IEnumerable<Type> GetRegisteredTypes() => _typeToString.Keys;
    public string GetTypeName(Type type)
    {
        bool registered = _typeToString.TryGetValue(type, out string? name);

        return registered ? name! : throw new ArgumentException($"The type '{type}' is not registered.");
    }
    public string GetTypeName<TType>()
        where TType : TBaseType => GetTypeName(typeof(TType));

    public Type GetTypeByName(string name)
    {
        bool registered = _stringToType.TryGetValue(name, out Type? type);

        return registered ? type! : throw new ArgumentException($"No type was registered by the name of '{name}'.");
    }

    private static string getNameOfType(Type type)
    {
        NamedTypeAttribute? attribute = type.GetCustomAttribute<NamedTypeAttribute>();

        string result = attribute?.Name ?? type.Name;
        return result;
    }
}
