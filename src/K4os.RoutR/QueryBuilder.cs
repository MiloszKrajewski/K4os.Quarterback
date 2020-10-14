using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.RoutR
{
	public readonly struct QueryBuilder<TResponse>
	{
		private readonly IServiceProvider _provider;

		public QueryBuilder(IServiceProvider provider)
		{
			_provider = provider.Required(nameof(provider));
		}

		private IServiceProvider Provider => _provider.Required(nameof(Provider));

		public Task<TResponse> Query<TQuery>(TQuery query, CancellationToken token = default) => 
			Provider.Query<TQuery, TResponse>(query, token);
	}
}
