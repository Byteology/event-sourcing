namespace Byteology.EventSourcing.EntityFramework;

using System.Text.Json;

public class DefaultSerializer<TBase> : ISerializer<TBase>
{
    public virtual string Serialize(TBase obj)
    {
        JsonSerializerOptions serializationOptions = new(JsonSerializerDefaults.Web);
        string result = JsonSerializer.Serialize(obj, serializationOptions);
        return result;
    }

    public virtual TBase Deserialize(Type type, string data)
    {
        if (!type.IsAssignableTo(typeof(TBase)))
            throw new ArgumentException($"The type '{type}' should be derived from '{typeof(TBase)}'.");

        JsonSerializerOptions serializationOptions = new(JsonSerializerDefaults.Web);

        if (JsonSerializer.Deserialize(data, type, serializationOptions) is not TBase result)
            throw new InvalidOperationException($"Unable to deserialize object of type '{type}'.");

        return result;
    }
}
