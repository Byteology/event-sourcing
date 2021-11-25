namespace Byteology.EventSourcing.Configuration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NamedTypeAttribute : Attribute
{
    public string Name { get; }

    public NamedTypeAttribute(string name)
    {
        Name = name;
    }
}
