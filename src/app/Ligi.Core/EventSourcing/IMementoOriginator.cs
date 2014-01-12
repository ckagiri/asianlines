namespace Ligi.Core.EventSourcing
{
    public interface IMementoOriginator
    {
        IMemento SaveToMemento();
    }

    public interface IMemento
    {
        int Version { get; }
    }
}
