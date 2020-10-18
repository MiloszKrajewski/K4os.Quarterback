using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Commands
{
	public class GenericCommandHandler<TCommand>:
		LoggingHandler, ICommandHandler<TCommand>
	{
		public GenericCommandHandler(ILog log): base(log) { }

		public Task Handle(TCommand command, CancellationToken token)
		{
			var commandType = typeof(TCommand).GetFriendlyName();
			var actualType = command.GetType().GetFriendlyName();
			Log($"GenericCommandHandler<{commandType}>({actualType})");
			return Task.CompletedTask;
		}
	}
}
