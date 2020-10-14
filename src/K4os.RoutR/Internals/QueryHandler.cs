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
	public static class QueryHandler
	{
		private const BindingFlags DefaultBindingFlags =
			BindingFlags.Static | BindingFlags.NonPublic;

		public static Task<object> Query(
			IServiceProvider provider,
			Type queryType, Type responseType, object query,
			CancellationToken token)
		{
			var handlerType = GetHandlerType(queryType, responseType);
			var handler = provider.GetRequiredService(handlerType);
			var handlerInvoker = GetHandlerInvoker(queryType, responseType);
			var actualHandlerType = handler.GetType();
			var pipelineType = GetPipelineType(actualHandlerType, queryType, responseType);
			var pipeline = provider.GetServices(pipelineType).AsArray();
			return Execute(pipelineType, pipeline, handler, handlerInvoker, query, token);
		}

		public static Task<object> Query<TQuery>(
			IServiceProvider provider, Type queryType, TQuery query, CancellationToken token)
		{
			var handlerType = GetHandlerType(queryType);
			var handler = provider.GetRequiredService(handlerType);
			var actualHandlerType = handler.GetType();
			var handlerInfo = GetHandlerInfo(queryType, actualHandlerType);
			var handlerInvoker = handlerInfo.Invoker;
			var responseType = handlerInfo.ResponseType;
			var pipelineType = GetPipelineType(actualHandlerType, queryType, responseType);
			var pipeline = provider.GetServices(pipelineType).AsArray();
			return Execute(pipelineType, pipeline, handler, handlerInvoker, query, token);
		}

		private static Task<object> Execute<TQuery>(
			Type pipelineType, object[] pipeline,
			object handler, HandlerInvoker handlerInvoker, TQuery query,
			CancellationToken token)
		{
			if (pipeline.Length <= 0)
				return handlerInvoker(handler, query, token);

			Func<Task<object>> next = () => handlerInvoker(handler, query, token);
			var pipelineInvoker = GetPipelineInvoker(pipelineType);

			Func<Task<object>> Combine(object wrapper, Func<Task<object>> rest) =>
				() => pipelineInvoker(wrapper, handler, query, rest, token);

			for (var i = pipeline.Length - 1; i >= 0; i--)
				next = Combine(pipeline[i], next);

			return next();
		}

		private static readonly ConcurrentDictionary<(Type, Type), Type> HandlerTypes2 =
			new ConcurrentDictionary<(Type, Type), Type>();

		private static Type GetHandlerType(Type queryType, Type responseType) =>
			HandlerTypes2.GetOrAdd((queryType, responseType), NewHandlerType);

		private static Type NewHandlerType((Type queryType, Type responseType) types) =>
			typeof(IQueryHandler<,>).MakeGenericType(types.queryType, types.responseType);

		private static readonly ConcurrentDictionary<Type, Type> HandlerTypes1 =
			new ConcurrentDictionary<Type, Type>();

		private static Type GetHandlerType(Type queryType) =>
			HandlerTypes1.GetOrAdd(queryType, NewHandlerType);

		private static Type NewHandlerType(Type queryType) =>
#pragma warning disable 618
			typeof(IQueryHandler<>).MakeGenericType(queryType);
#pragma warning restore 618

		private delegate Task<object> HandlerInvoker(
			object handler, object query, CancellationToken token);

		private class HandlerInfo
		{
			public Type QueryType { get; }
			public int QueryDistance { get; }
			public Type ResponseType { get; }
			public HandlerInvoker Invoker { get; set; }

			public HandlerInfo(Type queryType, int queryDistance, Type responseType)
			{
				QueryType = queryType;
				QueryDistance = queryDistance;
				ResponseType = responseType;
			}
		}

		private static readonly ConcurrentDictionary<(Type, Type), HandlerInfo> HandlerInfos =
			new ConcurrentDictionary<(Type, Type), HandlerInfo>();

		private static HandlerInfo GetHandlerInfo(Type queryType, Type actualHandlerType) =>
			HandlerInfos.GetOrAdd((queryType, actualHandlerType), NewHandlerInfo);

		private static HandlerInfo NewHandlerInfo((Type, Type) types)
		{
			var (queryType, actualHandlerType) = types;

			HandlerInfo TryMatchRequest(Type interfaceType)
			{
				if (!interfaceType.IsGenericType)
					return null;
				if (interfaceType.GetGenericTypeDefinition() != typeof(IQueryHandler<,>))
					return null;

				var genericArgs = interfaceType.GetGenericArguments();
				var declaredQueryType = genericArgs[0];

				if (!queryType.InheritsFrom(declaredQueryType))
					return null;

				var distance = queryType.DistanceFrom(declaredQueryType);
				var responseType = genericArgs[1];

				return new HandlerInfo(declaredQueryType, distance, responseType);
			}

			var match = actualHandlerType
				.GetInterfaces()
				.Select(TryMatchRequest)
				.Where(m => m != null)
				.MinBy(r => r.QueryDistance);

			if (match is null)
				throw new ArgumentException(
					string.Format(
						"No matching handler could be found for {0} in {1}",
						queryType.GetFriendlyName(),
						actualHandlerType.GetFriendlyName()));

			match.Invoker = GetHandlerInvoker(match.QueryType, match.ResponseType);

			return match;
		}

		private static readonly ConcurrentDictionary<(Type, Type), HandlerInvoker> HandlerInvokers =
			new ConcurrentDictionary<(Type, Type), HandlerInvoker>();

		private static HandlerInvoker GetHandlerInvoker(Type queryType, Type responseType) =>
			HandlerInvokers.GetOrAdd((queryType, responseType), NewHandlerInvoker);

		private static Task<object> UntypedHandlerInvoker<TQuery, TResponse>(
			object handler, object query, CancellationToken token) =>
			((IQueryHandler<TQuery, TResponse>) handler).Handle((TQuery) query, token).AsObject();

		private static HandlerInvoker NewHandlerInvoker((Type, Type) types)
		{
			var (queryType, responseType) = types;
			var handlerArg = Expression.Parameter(typeof(object));
			var queryArg = Expression.Parameter(typeof(object));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var handleMethod = typeof(QueryHandler)
				.GetMethod(nameof(UntypedHandlerInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedHandlerInvoker))
				.MakeGenericMethod(queryType, responseType);
			var body =
				Expression.Call(handleMethod, handlerArg, queryArg, tokenArg);
			var lambda = Expression.Lambda<HandlerInvoker>(
				body, handlerArg, queryArg, tokenArg);
			return lambda.Compile();
		}

		private static readonly ConcurrentDictionary<(Type, Type, Type), Type> PipelineTypes =
			new ConcurrentDictionary<(Type, Type, Type), Type>();

		private static Type GetPipelineType(
			Type handlerType, Type queryType, Type responseType) =>
			PipelineTypes.GetOrAdd((handlerType, queryType, responseType), NewPipelineType);

		private static Type NewPipelineType((Type, Type, Type) types)
		{
			var (handlerType, queryType, responseType) = types;
			return typeof(IQueryPipeline<,,>).MakeGenericType(handlerType, queryType, responseType);
		}

		private delegate Task<object> PipelineInvoker(
			object wrapper,
			object handler, object query, Func<Task<object>> next,
			CancellationToken token);

		private static readonly ConcurrentDictionary<Type, PipelineInvoker> PipelineInvokers =
			new ConcurrentDictionary<Type, PipelineInvoker>();

		private static PipelineInvoker GetPipelineInvoker(Type pipelineType) =>
			PipelineInvokers.GetOrAdd(pipelineType, NewPipelineInvoker);

		private static Task<object> UntypedPipelineInvoker<THandler, TQuery, TResponse>(
			object wrapper,
			object handler, object query, Func<Task<object>> next,
			CancellationToken token) where THandler: IQueryHandler<TQuery, TResponse> =>
			((IQueryPipeline<THandler, TQuery, TResponse>) wrapper).Handle(
				(THandler) handler, (TQuery) query, () => next().As<TResponse>(), token).AsObject();

		private static PipelineInvoker NewPipelineInvoker(Type pipelineType)
		{
			var args = pipelineType.GetGenericArguments();
			var (handlerType, queryType, responseType) = (args[0], args[1], args[2]);
			var wrapperArg = Expression.Parameter(typeof(object));
			var handlerArg = Expression.Parameter(typeof(object));
			var queryArg = Expression.Parameter(typeof(object));
			var nextArg = Expression.Parameter(typeof(Func<Task<object>>));
			var tokenArg = Expression.Parameter(typeof(CancellationToken));
			var method = typeof(QueryHandler)
				.GetMethod(nameof(UntypedPipelineInvoker), DefaultBindingFlags)
				.Required(nameof(UntypedPipelineInvoker))
				.MakeGenericMethod(handlerType, queryType, responseType);
			var body = Expression.Call(
				method, wrapperArg, handlerArg, queryArg, nextArg, tokenArg);
			var lambda = Expression.Lambda<PipelineInvoker>(
				body, wrapperArg, handlerArg, queryArg, nextArg, tokenArg);
			return lambda.Compile();
		}
	}
}
