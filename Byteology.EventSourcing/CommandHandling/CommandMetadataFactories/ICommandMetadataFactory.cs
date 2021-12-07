namespace Byteology.EventSourcing.CommandHandling.CommandMetadataFactories;

public interface ICommandMetadataFactory
{
    CommandMetadata CreateCommandMetadata();
}
