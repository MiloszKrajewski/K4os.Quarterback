using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using K4os.RoutR.Abstractions;
using Xunit;

namespace K4os.RoutR.Test
{
	public class UnitTest1
	{
		[Fact]
		public async Task Test1()
		{
			var container = ConfigureProvider();
			var command = new CommandA();
			await container.Send(command);
			Assert.Equal(new[] {
				"CommandAHandlerPipeline(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)"
			}, command.Log);
		}
		
		[Fact]
		public async Task Test2()
		{
			var container = ConfigureProvider();
			var command = new CommandA();
			await container.SendAny(command);
			Assert.Equal(new[] {
				"CommandAHandlerPipeline(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)"
			}, command.Log);
		}


		private static IServiceProvider ConfigureProvider()
		{
			var container = new Container();
			container.RegisterMany(
				new[] {
					typeof(CommandAHandler),
					typeof(CommandAHandlerPipeline)
				});
			return container;
		}
	}

	public class CommandA
	{
		public List<string> Log = new List<string>();
	}

	public class CommandAHandler: ICommandHandler<CommandA>
	{
		public Task Handle(CommandA command, CancellationToken token)
		{
			command.Log.Add("CommandAHandler(CommandA)");
			return Task.CompletedTask;
		}
	}

	public class CommandAHandlerPipeline: ICommandPipeline<CommandAHandler, CommandA>
	{
		public Task Handle(
			CommandAHandler handler, CommandA command, Func<Task> next, CancellationToken token)
		{
			command.Log.Add("CommandAHandlerPipeline(CommandAHandler,CommandA)");
			return next();
		}
	}
}
