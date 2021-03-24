using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Internals;

namespace K4os.RoutR
{
	/// <summary>
	/// Extension methods for service provider to handle requests.
	/// </summary>
	public static class RequestExtensions
	{
		/// <summary>
		/// Calls handler with given request object.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Request type (determines handler).</typeparam>
		/// <typeparam name="TResponse">Response type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		public static Task<TResponse> Request<TRequest, TResponse>(
			this IServiceProvider provider, TRequest request, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			request.Required(nameof(request));
			return RequestHandler
				.Request(provider, typeof(TRequest), typeof(TResponse), request!, token)
				.As<TResponse>();
		}

		/// <summary>
		/// Calls handler with given request object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Request type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		public static Task<object> Request<TRequest>(
			this IServiceProvider provider, TRequest request, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			request.Required(nameof(request));
			return RequestHandler.Request(provider, typeof(TRequest), request, token);
		}

		/// <summary>
		/// Calls handler with given request object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="request">Request (its type determines handler).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Response.</returns>
		public static Task<object> RequestAny(
			this IServiceProvider provider, object request, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			request.Required(nameof(request));
			return RequestHandler.Request(provider, request.GetType(), request, token);
		}

		/// <summary>
		/// Creates request builder asserting response type which allows a little bit
		/// more fluent requesting.
		/// </summary>
		/// <param name="provider">Service provider.</param>
		/// <typeparam name="TResponse">Expected response type.</typeparam>
		/// <returns>Request builder.</returns>
		public static RequestBuilder<TResponse> Expecting<TResponse>(
			this IServiceProvider provider) =>
			new RequestBuilder<TResponse>(provider);
	}
}
