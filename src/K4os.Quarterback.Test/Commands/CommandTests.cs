using System;
using System.Threading.Tasks;
using DryIoc;
using K4os.Quarterback;
using K4os.Quarterback.Abstractions;
using Xunit;

namespace K4os.RoutR.Test.Commands
{
	public class CommandTests
	{
		private readonly Container _container = new Container();

		[Fact]
		public async Task CommandHandlerIsCalled()
		{
			Configure(typeof(CommandAHandler));
			var command = new CommandA();
			await _container.Send(command);
			Expect("CommandAHandler(CommandA)");
		}

		[Fact]
		public async Task CommandHandlerIsCalledInsidePipeline()
		{
			Configure(typeof(CommandAHandler), typeof(CommandAHandlerPipeline));
			var command = new CommandA();
			await _container.Send(command);
			Expect(
				"CommandAHandlerPipeline(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)"
			);
		}

		[Fact]
		public async Task CommandHandlerIsCalledWhenDynamicallyInvoked()
		{
			Configure(typeof(CommandAHandler), typeof(CommandAHandlerPipeline));
			var command = new CommandA();
			await _container.SendAny(command);
			Expect(
				"CommandAHandlerPipeline(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)"
			);
		}

		[Fact]
		public async Task CommandHandlerIsCalledInsideWrapper()
		{
			Configure(typeof(CommandAHandler), typeof(CommandAHandlerWrapper));
			var command = new CommandA();
			await _container.Send(command);
			Expect(
				"CommandAHandlerWrapper.In(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)",
				"CommandAHandlerWrapper.Out(CommandAHandler,CommandA)"
			);
		}

		[Fact]
		public async Task MultipleWrappersAreCalled()
		{
			Configure(
				typeof(CommandAHandler),
				typeof(CommandAHandlerWrapper),
				typeof(CommandAHandlerPipeline)
			);
			var command = new CommandA();
			await _container.Send(command);
			Expect(
				"CommandAHandlerWrapper.In(CommandAHandler,CommandA)",
				"CommandAHandlerPipeline(CommandAHandler,CommandA)",
				"CommandAHandler(CommandA)",
				"CommandAHandlerWrapper.Out(CommandAHandler,CommandA)"
			);
		}

		[Fact]
		public async Task GenericCommandHandlerIsUsedWhenThereIsNoDedicatedHandler()
		{
			Configure(
				typeof(GenericCommandHandler<>),
				typeof(CommandAHandlerPipeline)
			);
			var command = new CommandA();
			await _container.Send(command);
			Expect("GenericCommandHandler<CommandA>(CommandA)");
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task DedicatedCommandHandlerWinsWithGenericHandler(bool reverse)
		{
			var services = new[] { typeof(CommandAHandler), typeof(GenericCommandHandler<>) };
			if (reverse) Array.Reverse(services);
			Configure(services);
			var command = new CommandA();
			await _container.Send(command);
			Expect("CommandAHandler(CommandA)");
		}

		[Fact]
		public async Task ConstrainedGenericHandlerIsUsedWhenTheIsNoDirectHandler()
		{
			Configure(typeof(ConstrainedCommandHandler<>));
			var command = new CommandB();
			await _container.Send(command);
			Expect("ConstrainedCommandHandler<CommandB>(CommandB)");
		}

		[Fact]
		public async Task ConstraintsAsRespected()
		{
			Configure(typeof(ConstrainedCommandHandler<>));
			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _container.Send(new CommandA()));
			await _container.Send(new CommandB());
		}
		
		[Fact]
		public async Task HandlerForBaseClassIsUsedIfNoDirectHandler()
		{
			Configure();
			// NOTE: DryIoC requires different registration technique to register as covariant
			// https://stackoverflow.com/questions/48302172/covariant-service-resolution-by-ioc-container
			// using constrained generic might be an easier option 
			_container.Register(typeof(ICommandHandler<>), typeof(CommandAHandler));
			var command = new CommandB();
			await _container.Send(command);
			Expect("CommandAHandler(CommandB)");
		}

		// With many handlers closer match is picked up

		private void Configure(params Type[] types)
		{
			_container.RegisterInstance<ILog>(new Log());
			_container.RegisterMany(types);
		}

		private void Expect(params string[] messages)
		{
			var snapshot = _container.Resolve<ILog>().Snapshot;
			Assert.Equal(messages, snapshot);
		}
	}
}
