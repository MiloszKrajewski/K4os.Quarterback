using System.Threading;
using System.Threading.Tasks;

namespace K4os.Quarterback.Abstractions
{
	/// <summary>
	/// Half-defined request, we already known expected response type, now we need request type.
	/// </summary>
	/// <typeparam name="TResponse">Preconfigured response type.</typeparam>
	public interface IRequestBuilder<TResponse>
	{
		/// <summary>
		/// Calls handler with given request object.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Request type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		Task<TResponse> Request<TRequest>(TRequest request, CancellationToken token = default);
	}
}
