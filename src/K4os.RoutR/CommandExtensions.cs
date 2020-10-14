using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.RoutR
{
	public static class CommandExtensions
	{
		public static Task Send<TCommand>(
			this IServiceProvider provider, TCommand command, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			command.Required(nameof(command));
			return CommandHandler.Send(provider, typeof(TCommand), command, token);
		}

		public static Task SendAny(
			this IServiceProvider provider, object command, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			command.Required(nameof(command));
			return CommandHandler.Send(provider, command.GetType(), command, token);
		}
	}
}
