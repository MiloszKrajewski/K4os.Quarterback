using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.RoutR.Abstractions
{
	public interface ICommandHandler<in TCommand>
	{
		Task Handle(TCommand command, CancellationToken token);
	}

	public interface IEventHandler<in TEvent>
	{
		Task Handle(TEvent @event, CancellationToken token);
	}

	[Obsolete("Use IQueryHandle<TQuery, TResponse> instead")]
	public interface IQueryHandler<in TQuery> { }

	public interface IQueryHandler<in TQuery, TResponse>:
#pragma warning disable 618
		IQueryHandler<TQuery>
#pragma warning restore 618
	{
		Task<TResponse> Handle(TQuery query, CancellationToken token);
	}

	public interface ICommandPipeline<in THandler, in TCommand>
		where THandler: ICommandHandler<TCommand>
	{
		Task Handle(
			THandler handler, TCommand command, Func<Task> next, CancellationToken token);
	}

	public interface IEventPipeline<in THandler, in TEvent>
		where THandler: IEventHandler<TEvent>
	{
		Task Handle(
			THandler handler, TEvent @event, Func<Task> next, CancellationToken token);
	}

	public interface IQueryPipeline<in THandler, in TQuery, TResponse>
		where THandler: IQueryHandler<TQuery, TResponse>
	{
		Task<TResponse> Handle(
			THandler handler, TQuery query, Func<Task<TResponse>> next, CancellationToken token);
	}
}
