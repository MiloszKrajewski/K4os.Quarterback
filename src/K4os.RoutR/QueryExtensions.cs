using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Internals;

namespace K4os.RoutR
{
	/// <summary>
	/// Extension methods for service provider to handle queries.
	/// </summary>
	public static class QueryExtensions
	{
		/// <summary>
		/// Queries handler with given query object.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="query">Query.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TQuery">Query type (determines handler).</typeparam>
		/// <typeparam name="TResponse">Response type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		public static Task<TResponse> Query<TQuery, TResponse>(
			this IServiceProvider provider, TQuery query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler
				.Query(provider, typeof(TQuery), typeof(TResponse), query, token)
				.As<TResponse>();
		}

		/// <summary>
		/// Queries handler with given query object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="query">Query.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TQuery">Query type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		public static Task<object> Query<TQuery>(
			this IServiceProvider provider, TQuery query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler.Query(provider, typeof(TQuery), query, token);
		}

		/// <summary>
		/// Queries handler with given query object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="query">Query (its type determines handler).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Response.</returns>
		public static Task<object> QueryAny(
			this IServiceProvider provider, object query, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			query.Required(nameof(query));
			return QueryHandler.Query(provider, query.GetType(), query, token);
		}

		/// <summary>
		/// Creates query builder asserting response type which allows a little bit
		/// more fluent querying.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <typeparam name="TResponse">Expected response type.</typeparam>
		/// <returns>Query builder.</returns>
		public static QueryBuilder<TResponse> Expecting<TResponse>(
			this IServiceProvider provider) =>
			new QueryBuilder<TResponse>(provider);
	}
}
