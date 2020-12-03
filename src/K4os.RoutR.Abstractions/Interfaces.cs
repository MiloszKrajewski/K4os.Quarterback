using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable UnusedTypeParameter

namespace K4os.RoutR.Abstractions
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

	/// <summary>Command pipeline processor.</summary>
	/// <typeparam name="THandler">Wrapped handler.</typeparam>
	/// <typeparam name="TCommand">Handled command.</typeparam>
	public interface ICommandPipeline<in THandler, in TCommand>
		where THandler: ICommandHandler<TCommand>
	{
		/// <summary>
		/// Wraps execution of a handler. Please note, you need to call <c>next()</c>
		/// to continue execution of this chain.
		/// </summary>
		/// <param name="handler">Handler being executed.</param>
		/// <param name="command">Command being handled.</param>
		/// <param name="next">Next chunk in processing pipeline.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Completed when processing is done.</returns>
		Task Handle(THandler handler, TCommand command, Func<Task> next, CancellationToken token);
	}

	/// <summary>Event pipeline processor.</summary>
	/// <typeparam name="THandler">Wrapped handler.</typeparam>
	/// <typeparam name="TEvent">Handled event.</typeparam>
	public interface IEventPipeline<in THandler, in TEvent>
		where THandler: IEventHandler<TEvent>
	{
		/// <summary>
		/// Wraps execution of a handler. Please note, you need to call <c>next()</c>
		/// to continue execution of this chain.
		/// </summary>
		/// <param name="handler">Handler being executed.</param>
		/// <param name="event">Event being handled.</param>
		/// <param name="next">Next chunk in processing pipeline.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Completed when processing is done.</returns>
		Task Handle(THandler handler, TEvent @event, Func<Task> next, CancellationToken token);
	}

	/// <summary>
	/// Request processing pipeline.
	/// </summary>
	/// <typeparam name="THandler">Wrapped handler.</typeparam>
	/// <typeparam name="TRequest">Handled request.</typeparam>
	/// <typeparam name="TResponse">Request response.</typeparam>
	public interface IRequestPipeline<in THandler, in TRequest, TResponse>
		where THandler: IRequestHandler<TRequest, TResponse>
	{
		/// <summary>
		/// Wraps execution of a handler. Please note, you need to call <c>next()</c>
		/// to continue execution of this chain.
		/// </summary>
		/// <param name="handler">Handler being executed.</param>
		/// <param name="request">Request being handled.</param>
		/// <param name="next">Next chunk in processing pipeline.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Result of execution.</returns>
		Task<TResponse> Handle(
			THandler handler, TRequest request, Func<Task<TResponse>> next,
			CancellationToken token);
	}
}
