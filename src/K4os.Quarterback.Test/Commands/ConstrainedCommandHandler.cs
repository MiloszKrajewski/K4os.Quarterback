using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.RoutR.Test.Commands
{
	public class ConstrainedCommandHandler<TCommand>:
		LoggingHandler, ICommandHandler<TCommand>
		where TCommand: CommandB
	{
		public ConstrainedCommandHandler(ILog log): base(log) { }

		public Task Handle(TCommand command, CancellationToken token)
		{
			var commandType = typeof(TCommand).GetFriendlyName();
			var actualType = command.GetType().GetFriendlyName();
			Log($"ConstrainedCommandHandler<{commandType}>({actualType})");
			return Task.CompletedTask;
		}
	}
}
