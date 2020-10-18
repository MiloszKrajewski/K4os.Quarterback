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

	/// <summary>Marker interface for query handlers, do no use directly.</summary>
	/// <typeparam name="TQuery">Type of query.</typeparam>
	[Obsolete("Use IQueryHandle<TQuery, TResponse> instead")]
	public interface IQueryHandler<in TQuery> { }

	/// <summary>Interface for query handlers.</summary>
	/// <typeparam name="TQuery">Type of query.</typeparam>
	/// <typeparam name="TResponse">Type of response.</typeparam>
	public interface IQueryHandler<in TQuery, TResponse>:
#pragma warning disable 618
		IQueryHandler<TQuery>
#pragma warning restore 618
	{
		/// <summary>Query handler.</summary>
		/// <param name="query">Query.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Result of execution.</returns>
		Task<TResponse> Handle(TQuery query, CancellationToken token);
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
	/// Query processing pipeline.
	/// </summary>
	/// <typeparam name="THandler">Wrapped handler.</typeparam>
	/// <typeparam name="TQuery">Handled query.</typeparam>
	/// <typeparam name="TResponse">Query response.</typeparam>
	public interface IQueryPipeline<in THandler, in TQuery, TResponse>
		where THandler: IQueryHandler<TQuery, TResponse>
	{
		/// <summary>
		/// Wraps execution of a handler. Please note, you need to call <c>next()</c>
		/// to continue execution of this chain.
		/// </summary>
		/// <param name="handler">Handler being executed.</param>
		/// <param name="query">Query being handled.</param>
		/// <param name="next">Next chunk in processing pipeline.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>Result of execution.</returns>
		Task<TResponse> Handle(
			THandler handler, TQuery query, Func<Task<TResponse>> next, CancellationToken token);
	}
}
