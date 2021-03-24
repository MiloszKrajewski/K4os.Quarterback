using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.RoutR.Test.Commands
{
	public class CommandAHandler: LoggingHandler, ICommandHandler<CommandA>
	{
		public CommandAHandler(ILog log): base(log) { }

		public Task Handle(CommandA command, CancellationToken token)
		{
			var actualCommandType = command.GetType().GetFriendlyName();
			Log($"CommandAHandler({actualCommandType})");
			return Task.CompletedTask;
		}
	}
}
