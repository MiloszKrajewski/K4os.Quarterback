using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Internals;

namespace K4os.Quarterback
{
	/// <summary>
	/// Extension methods for service provider to handle commands.
	/// </summary>
	public static class CommandExtensions
	{
		/// <summary>Send a command to registered handler.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="command">Command.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TCommand">Type of command (determines handler).</typeparam>
		/// <returns>When handling is finished.</returns>
		public static Task Send<TCommand>(
			this IServiceProvider provider, TCommand command, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			command.Required(nameof(command));
			return CommandHandler.Send(provider, typeof(TCommand), command!, token);
		}

		/// <summary>Send a command to registered handler.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="command">Command (its type determines handler).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>When handling is finished.</returns>
		public static Task SendAny(
			this IServiceProvider provider, object command, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			command.Required(nameof(command));
			return CommandHandler.Send(provider, command.GetType(), command, token);
		}
	}
}
