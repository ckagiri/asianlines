namespace AsianLines.Core.Events
{
    public interface IEventHandlerRegistry
    {
        void Register(IEventHandler handler);
    }
}
