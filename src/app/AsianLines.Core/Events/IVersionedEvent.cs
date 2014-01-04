namespace AsianLines.Core.Events
{
    public interface IVersionedEvent : IEvent
    {
        int Version { get; }
    }
}
