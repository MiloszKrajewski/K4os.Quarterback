using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.Quarterback.Abstractions
{
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
