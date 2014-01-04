namespace Ligi.Core.Commands
{
	public interface ICommandHandler { }

	public interface ICommandHandler<T> : ICommandHandler
		where T : ICommand
	{
		void Handle(T command);
	}
}
