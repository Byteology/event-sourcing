namespace Byteology.EventSourcing.EntityFramework;

using System.Reflection;

public class TypeRegistry<TBase>
    where TBase : class
{
    private readonly Dictionary<Type, string> _typeToString = new();
    private readonly Dictionary<string, Type> _stringToType = new();

    public void RegisterType(Type type)
    {
        if (!type.IsAssignableTo(typeof(TBase)))
            throw new ArgumentException($"The specified type can't be assigned to {typeof(TBase)}.");

        if (_typeToString.ContainsKey(type))
            return;

        string name = GetNameOfType(type);

        if (_stringToType.ContainsKey(name))
            throw new InvalidOperationException($"Another type with the same name was already registered.");

        _typeToString.Add(type, name);
        _stringToType.Add(name, type);
    }
    public void RegisterType<T>()
        where T : TBase => RegisterType(typeof(T));

    public void RegisterTypes(IEnumerable<Type> types)
    {
        foreach (Type type in types)
            RegisterType(type);
    }

    public void RegisterTypesInAssembly(Assembly assembly)
    {
        IEnumerable<Type> allTypes = assembly.GetTypes();
        foreach (Type type in allTypes)
            if (type.IsAssignableTo(typeof(TBase)))
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
    public Type GetTypeByName(string name)
    {
        bool registered = _stringToType.TryGetValue(name, out Type? type);

        return registered ? type! : throw new ArgumentException($"No type was registered by the name of '{name}'.");
    }

    protected virtual string GetNameOfType(Type type)
        => type.Name;
}
