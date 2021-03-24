using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.Quarterback
{
	/// <summary>
	/// Helper class to build request and specify expected result type.
	/// </summary>
	/// <typeparam name="TResponse"></typeparam>
	public readonly struct RequestBuilder<TResponse>
	{
		private readonly IServiceProvider _provider;

		internal RequestBuilder(IServiceProvider provider)
		{
			_provider = provider.Required(nameof(provider));
		}

		private IServiceProvider Provider => _provider.Required(nameof(Provider));

		/// <summary>Calls registered request handler.</summary>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Type of request (determines handler)</typeparam>
		/// <returns>Handler's response.</returns>
		public Task<TResponse> Request<TRequest>(
			TRequest request, CancellationToken token = default) => 
			Provider.Request<TRequest, TResponse>(request, token);
	}
}
