namespace AsianLines.Core.Commands
{
    public interface ICommandHandlerRegistry
    {
        void Register(ICommandHandler handler);
    }
}
