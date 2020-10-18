using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Commands
{
	public class CommandAHandlerPipeline: 
		LoggingHandler, ICommandPipeline<CommandAHandler, CommandA>
	{
		public CommandAHandlerPipeline(ILog log): base(log) { }

		public Task Handle(
			CommandAHandler handler, CommandA command, Func<Task> next, CancellationToken token)
		{
			Log("CommandAHandlerPipeline(CommandAHandler,CommandA)");
			return next();
		}
	}

	public class CommandAHandlerWrapper: 
		LoggingHandler, ICommandPipeline<CommandAHandler, CommandA>
	{
		public CommandAHandlerWrapper(ILog log): base(log) { }

		public async Task Handle(
			CommandAHandler handler, CommandA command, Func<Task> next, CancellationToken token)
		{
			Log("CommandAHandlerWrapper.In(CommandAHandler,CommandA)");
			await next();
			Log("CommandAHandlerWrapper.Out(CommandAHandler,CommandA)");
		}
	}
}
