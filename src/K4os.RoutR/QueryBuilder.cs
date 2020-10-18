using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.RoutR
{
	/// <summary>
	/// Helper class to build query and specify expected result type.
	/// </summary>
	/// <typeparam name="TResponse"></typeparam>
	public readonly struct QueryBuilder<TResponse>
	{
		private readonly IServiceProvider _provider;

		internal QueryBuilder(IServiceProvider provider)
		{
			_provider = provider.Required(nameof(provider));
		}

		private IServiceProvider Provider => _provider.Required(nameof(Provider));

		/// <summary>
		/// Queries registered handler.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TQuery">Type of query (determines handler)</typeparam>
		/// <returns>Handler's response.</returns>
		public Task<TResponse> Query<TQuery>(TQuery query, CancellationToken token = default) => 
			Provider.Query<TQuery, TResponse>(query, token);
	}
}
