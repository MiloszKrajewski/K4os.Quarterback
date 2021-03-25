using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.Quarterback.Abstractions
{
	/// <summary>
	/// Abstraction over mediator pattern.
	/// </summary>
	public interface IMediator
	{
		/// <summary>Send a command to registered handler.</summary>
		/// <param name="command">Command.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TCommand">Type of command (determines handler).</typeparam>
		/// <returns>When handling is finished.</returns>
		Task Send<TCommand>(TCommand command, CancellationToken token = default);

		/// <summary>Send a command to registered handler.</summary>
		/// <param name="command">Command (its type determines handler).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>When handling is finished.</returns>
		Task SendAny(object command, CancellationToken token = default);

		/// <summary>Publishes event to all registered compatible handlers.</summary>
		/// <param name="event">Event.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TEvent">Type of event (determines handlers).</typeparam>
		/// <returns>When handling is finished.</returns>
		Task Publish<TEvent>(TEvent @event, CancellationToken token = default);

		/// <summary>Publishes event to all registered compatible handlers.</summary>
		/// <param name="event">Event (its type determines handlers).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>When handling is finished.</returns>
		Task PublishAny(object @event, CancellationToken token = default);

		/// <summary>
		/// Calls handler with given request object.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Request type (determines handler).</typeparam>
		/// <typeparam name="TResponse">Response type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		Task<TResponse> Request<TRequest, TResponse>(
			TRequest request, CancellationToken token = default);

		/// <summary>
		/// Calls handler with given request object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TRequest">Request type (determines handler).</typeparam>
		/// <returns>Response.</returns>
		Task<object> Request<TRequest>(
			TRequest request, CancellationToken token = default);

		/// <summary>
		/// Calls handler with given request object. Please note, you do not specify what expected
		/// response is, so it will use first handler returned by service provider.
		/// </summary>
		/// <param name="request">Request (its type determines handler).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Response.</returns>
		Task<object> RequestAny(
			object request, CancellationToken token = default);

		/// <summary>
		/// Creates request builder asserting response type which allows a little bit
		/// more fluent requesting.
		/// </summary>
		/// <typeparam name="TResponse">Expected response type.</typeparam>
		/// <returns>Request builder.</returns>
		IRequestBuilder<TResponse> Expecting<TResponse>();
	}
}
