using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using K4os.Quarterback;
using K4os.Quarterback.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks
{
	public class CommandA: IRequest { }

	public class CommandAHandler:
		IRequestHandler<CommandA>,
		ICommandHandler<CommandA>
	{
		Task<Unit> MediatR.IRequestHandler<CommandA, Unit>.Handle(
			CommandA request, CancellationToken cancellationToken)
		{
			return Unit.Task;
		}

		Task ICommandHandler<CommandA>.Handle(CommandA command, CancellationToken token)
		{
			return Task.CompletedTask;
		}
	}

	public class Commands
	{
		private readonly Container _container = new Container();
		private object Resolve(Type type) => _container.Resolve(type);
		private static readonly CommandA Command = new();
		private static readonly object CommandAny = Command;

		[GlobalSetup]
		public void Setup()
		{
			_container.RegisterMany<CommandAHandler>();
			_container.Use<IServiceScopeFactory>(r => new DryIocServiceScopeFactory(r));
			_container.RegisterDelegate<ServiceFactory>(r => r.GetService);
			_container.Register<IMediator, Mediator>(Reuse.Scoped);
		}

		[Benchmark]
		public Task UseDynamicMediatR()
		{
			using var scope = _container.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
			return mediator.Send(CommandAny);
		}
		
		[Benchmark]
		public Task UseDynamicQuarterback()
		{
			using var scope = _container.CreateScope();
			var provider = scope.ServiceProvider;
			return provider.SendAny(CommandAny);
		}
		
		[Benchmark]
		public Task UseStaticMediatR()
		{
			using var scope = _container.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
			return mediator.Send(Command);
		}
		
		[Benchmark]
		public Task UseStaticQuarterback()
		{
			using var scope = _container.CreateScope();
			var provider = scope.ServiceProvider;
			return provider.SendAny(Command);
		}

	}
}
