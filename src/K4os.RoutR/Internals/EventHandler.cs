using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace K4os.RoutR.Internals
{
	internal static class EventHandler
	{
		private const BindingFlags DefaultBindingFlags =
			BindingFlags.Static | BindingFlags.NonPublic;

		public static Task Publish(
			IServiceProvider provider, Type eventType, object @event, CancellationToken token)
		{
			var handlerType = GetHandlerType(eventType);
			var handlers = provider.GetServices(handlerType);
			var handlerInvoker = GetHandlerInvoker(eventType);

			Task Handle(object handler) =>
				Execute(provider, eventType, handler, handlerInvoker, @event, token);

			return handlers.Select(Handle).WhenAll();
		}

		private static Task Execute(
			IServiceProvider provider,
			Type eventType,
			object handler, HandlerInvoker handlerInvoker, object @event,
			CancellationToken token)
		{
			var actualHandlerType = handler.GetType();
			var pipelineType = GetPipelineType(actualHandlerType, eventType);
			var pipeline = provider.GetServices(pipelineType).AsArray();
			return Execute(pipelineType, pipeline, handler, handlerInvoker, @event, token);
		}

		private static Task Execute(
			Type pipelineType, IReadOnlyList<object> pipeline,
			object handler, HandlerInvoker handlerInvoker, object @event,
			CancellationToken token)
		{
			if (pipeline.Count <= 0)
				return handlerInvoker(handler, @event, token);

			Func<Task> next = () => handlerInvoker(handler, @event, token);
			var pipelineInvoker = GetPipelineInvoker(pipelineType);

			Func<Task> Combine(object wrapper, Func<Task> rest) =>
				() => pipelineInvoker(wrapper, handler, @event, rest, token);

			for (var i = pipeline.Count - 1; i >= 0; i--)
				next = Combine(pipeline[i], next);

			return next();
		}

		private static readonly ConcurrentDictionary<Type, Type>
			HandlerTypes = new();

		private static Type GetHandlerType(Type eventType) =>
			HandlerTypes.GetOrAdd(eventType, NewHandleType);

		private static Type NewHandleType(Type eventType) =>
			typeof(IEventHandler<>).MakeGenericType(eventType);

		private static readonly ConcurrentDictionary<(Type, Type), Type>
			PipelineTypes = new();

		private static Type GetPipelineType(Type handlerType, Type eventType) =>
			PipelineTypes.GetOrAdd((handlerType, eventType), NewPipelineType);

		private static Type NewPipelineType((Type handlerType, Type eventType) types) =>
			typeof(IEventPipeline<,>).MakeGenericType(types.handlerType, types.eventType);

		private delegate Task HandlerInvoker(
			object handler, object @event, CancellationToken token);

		private static readonly ConcurrentDictionary<Type, HandlerInvoker>
			HandlerInvokers = new();

		private static HandlerInvoker GetHandlerInvoker(Type eventType) =>
			HandlerInvokers.GetOrAdd(eventType, NewHandlerInvoker);

		private static Task UntypedHandlerInvoker<TEvent>(
			object handler, object @event, CancellationToken token) =>
			((IEventHandler<TEvent>) handler).Handle((TEvent) @event, token);

		private static HandlerInvoker NewHandlerInvoker(Type eventType)
		{
			var handlerArg = Expression.Parameter(typeof(object));
			var eventArg = Expression.Parameter(typeof(object));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var method = typeof(EventHandler)
				.GetMethod(nameof(UntypedHandlerInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedHandlerInvoker))
				.MakeGenericMethod(eventType);
			var body = Expression.Call(
				method, handlerArg, eventArg, tokenArg);
			var lambda = Expression.Lambda<HandlerInvoker>(
				body, handlerArg, eventArg, tokenArg);
			return lambda.Compile();
		}

		private delegate Task PipelineInvoker(
			object wrapper,
			object handler, object @event, Func<Task> next,
			CancellationToken token);

		private static readonly ConcurrentDictionary<Type, PipelineInvoker>
			PipelineInvokers = new();

		private static PipelineInvoker GetPipelineInvoker(Type pipelineType) =>
			PipelineInvokers.GetOrAdd(pipelineType, NewPipelineInvoker);

		private static Task UntypedPipelineInvoker<THandler, TEvent>(
			object wrapper,
			object handler, object @event, Func<Task> next,
			CancellationToken token)
			where THandler: IEventHandler<TEvent> =>
			((IEventPipeline<THandler, TEvent>) wrapper)
			.Handle((THandler) handler, (TEvent) @event, next, token);

		private static PipelineInvoker NewPipelineInvoker(Type pipelineType)
		{
			var args = pipelineType.GetGenericArguments();
			var (handlerType, eventType) = (args[0], args[1]);
			var wrapperArg = Expression.Parameter(typeof(object));
			var handlerArg = Expression.Parameter(typeof(object));
			var eventArg = Expression.Parameter(typeof(object));
			var nextArg = Expression.Parameter(typeof(Func<Task>));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var method = typeof(EventHandler)
				.GetMethod(nameof(UntypedPipelineInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedPipelineInvoker))
				.MakeGenericMethod(handlerType, eventType);
			var body = Expression.Call(
				method, wrapperArg, handlerArg, eventArg, nextArg, tokenArg);
			var lambda = Expression.Lambda<PipelineInvoker>(
				body, wrapperArg, handlerArg, eventArg, nextArg, tokenArg);
			return lambda.Compile();
		}
	}
}
