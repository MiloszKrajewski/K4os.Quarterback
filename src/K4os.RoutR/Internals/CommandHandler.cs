using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace K4os.RoutR.Internals
{
	internal static class CommandHandler
	{
		private const BindingFlags DefaultBindingFlags =
			BindingFlags.Static | BindingFlags.NonPublic;

		public static Task Send(
			IServiceProvider provider, Type commandType, object command, CancellationToken token)
		{
			var handlerType = GetHandlerType(commandType);
			var handler = provider.GetRequiredService(handlerType);
			var handlerInvoker = GetHandlerInvoker(commandType);
			var pipelineType = GetPipelineType(handler.GetType(), commandType);
			var pipeline = provider.GetServices(pipelineType).AsArray();
			return Execute(pipelineType, pipeline, handler, handlerInvoker, command, token);
		}

		private static Task Execute(
			Type pipelineType, object[] pipeline,
			object handler, HandlerInvoker handlerInvoker, object command,
			CancellationToken token)
		{
			if (pipeline.Length <= 0)
				return handlerInvoker(handler, command, token);

			Func<Task> next = () => handlerInvoker(handler, command, token);
			var pipelineInvoker = GetPipelineInvoker(pipelineType);

			Func<Task> Combine(object wrapper, Func<Task> rest) =>
				() => pipelineInvoker(wrapper, handler, command, rest, token);

			for (var i = pipeline.Length - 1; i >= 0; i--)
				next = Combine(pipeline[i], next);

			return next();
		}

		private static readonly ConcurrentDictionary<Type, Type> HandlerTypes =
			new ConcurrentDictionary<Type, Type>();

		private static Type GetHandlerType(Type commandType) =>
			HandlerTypes.GetOrAdd(commandType, NewHandleType);

		private static Type NewHandleType(Type commandType) =>
			typeof(ICommandHandler<>).MakeGenericType(commandType);

		private static readonly ConcurrentDictionary<(Type, Type), Type> PipelineTypes =
			new ConcurrentDictionary<(Type, Type), Type>();

		private static Type GetPipelineType(Type handlerType, Type commandType) =>
			PipelineTypes.GetOrAdd((handlerType, commandType), NewPipelineType);

		private static Type NewPipelineType((Type handlerType, Type commandType) types) =>
			typeof(ICommandPipeline<,>).MakeGenericType(types.handlerType, types.commandType);

		private delegate Task HandlerInvoker(
			object handler, object command, CancellationToken token);

		private static readonly ConcurrentDictionary<Type, HandlerInvoker> HandlerInvokers =
			new ConcurrentDictionary<Type, HandlerInvoker>();

		private static HandlerInvoker GetHandlerInvoker(Type commandType) =>
			HandlerInvokers.GetOrAdd(commandType, NewHandlerInvoker);

		private static Task UntypedHandlerInvoker<TCommand>(
			object handler, object command, CancellationToken token) =>
			((ICommandHandler<TCommand>) handler).Handle((TCommand) command, token);

		private static HandlerInvoker NewHandlerInvoker(Type commandType)
		{
			var handlerArg = Expression.Parameter(typeof(object));
			var commandArg = Expression.Parameter(typeof(object));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var method = typeof(CommandHandler)
				.GetMethod(nameof(UntypedHandlerInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedHandlerInvoker))
				.MakeGenericMethod(commandType);
			var body = Expression.Call(
				method, handlerArg, commandArg, tokenArg);
			var lambda = Expression.Lambda<HandlerInvoker>(
				body, handlerArg, commandArg, tokenArg);
			return lambda.Compile();
		}

		private delegate Task PipelineInvoker(
			object wrapper,
			object handler, object command, Func<Task> next,
			CancellationToken token);

		private static readonly ConcurrentDictionary<Type, PipelineInvoker> PipelineInvokers =
			new ConcurrentDictionary<Type, PipelineInvoker>();

		private static PipelineInvoker GetPipelineInvoker(Type pipelineType) =>
			PipelineInvokers.GetOrAdd(pipelineType, NewPipelineInvoker);

		private static Task UntypedPipelineInvoker<THandler, TCommand>(
			object wrapper,
			object handler, object command, Func<Task> next,
			CancellationToken token)
			where THandler: ICommandHandler<TCommand> =>
			((ICommandPipeline<THandler, TCommand>) wrapper).Handle(
				(THandler) handler, (TCommand) command, next, token);

		private static PipelineInvoker NewPipelineInvoker(Type pipelineType)
		{
			var args = pipelineType.GetGenericArguments();
			var (handlerType, commandType) = (args[0], args[1]);
			var wrapperArg = Expression.Parameter(typeof(object));
			var handlerArg = Expression.Parameter(typeof(object));
			var commandArg = Expression.Parameter(typeof(object));
			var nextArg = Expression.Parameter(typeof(Func<Task>));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var method = typeof(CommandHandler)
				.GetMethod(nameof(UntypedPipelineInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedPipelineInvoker))
				.MakeGenericMethod(handlerType, commandType);
			var body = Expression.Call(
				method, wrapperArg, handlerArg, commandArg, nextArg, tokenArg);
			var lambda = Expression.Lambda<PipelineInvoker>(
				body, wrapperArg, handlerArg, commandArg, nextArg, tokenArg);
			return lambda.Compile();
		}
	}
}
