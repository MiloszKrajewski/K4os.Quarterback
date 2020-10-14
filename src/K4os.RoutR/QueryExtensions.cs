using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using K4os.RoutR.Internals;

namespace K4os.RoutR
{
	public static class QueryExtensions
	{
		public static Task<TResponse> Query<TQuery, TResponse>(
			this IServiceProvider provider, TQuery query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler
				.Query(provider, typeof(TQuery), typeof(TResponse), query, token)
				.As<TResponse>();
		}

		public static Task<object> Query<TQuery>(
			this IServiceProvider provider, TQuery query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler.Query(provider, typeof(TQuery), query, token);
		}

		public static Task<object> QueryAny(
			this IServiceProvider provider, object query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler.Query(provider, query.GetType(), query, token);
		}

		public static QueryBuilder<TResponse> Expecting<TResponse>(
			this IServiceProvider provider) =>
			new QueryBuilder<TResponse>(provider);
	}
}
