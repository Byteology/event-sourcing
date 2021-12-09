namespace Byteology.EventSourcing.EntityFramework;

public interface ISerializer<TBase>
{
    string Serialize(TBase obj);
    TBase Deserialize(Type type, string data);
}
