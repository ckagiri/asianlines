namespace Ligi.Core.Commands
{
    public interface ICommandHandlerRegistry
    {
        void Register(ICommandHandler handler);
    }
}
