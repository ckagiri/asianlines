namespace Ligi.Core.Events
{
    public interface IVersionedEvent : IEvent
    {
        int Version { get; }
    }
}
