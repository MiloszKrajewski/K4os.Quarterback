using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable UnusedTypeParameter

namespace K4os.Quarterback.Abstractions
{
	/// <summary>Interface for command handlers.</summary>
	/// <typeparam name="TCommand">Type of command.</typeparam>
	public interface ICommandHandler<in TCommand>
	{
		/// <summary>Command handler.</summary>
		/// <param name="command">Command.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Task indicating handling is finished.</returns>
		Task Handle(TCommand command, CancellationToken token);
	}

	/// <summary>Interface for event handlers.</summary>
	/// <typeparam name="TEvent">Type of event.</typeparam>
	public interface IEventHandler<in TEvent>
	{
		/// <summary>Event handler.</summary>
		/// <param name="event">Event.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Task indicating handling is finished.</returns>
		Task Handle(TEvent @event, CancellationToken token);
	}

	/// <summary>Marker interface for request handlers, do no use directly.</summary>
	/// <typeparam name="TRequest">Type of request.</typeparam>
	[Obsolete("Use IRequestHandler<TRequest, TResponse> instead")]
	public interface IRequestHandlerPartialMarker<in TRequest> { }

	/// <summary>Interface for request handlers.</summary>
	/// <typeparam name="TRequest">Type of request.</typeparam>
	/// <typeparam name="TResponse">Type of response.</typeparam>
	public interface IRequestHandler<in TRequest, TResponse>:
#pragma warning disable 618
		IRequestHandlerPartialMarker<TRequest>
#pragma warning restore 618
	{
		/// <summary>Request handler.</summary>
		/// <param name="request">Request.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Result of execution.</returns>
		Task<TResponse> Handle(TRequest request, CancellationToken token);
	}
}
