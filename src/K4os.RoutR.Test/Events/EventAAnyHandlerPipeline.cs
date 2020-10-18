using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Events
{
	public class EventAAnyHandlerPipeline<THandler>:
		LoggingHandler, IEventPipeline<THandler, EventA>
		where THandler: IEventHandler<EventA>
	{
		public EventAAnyHandlerPipeline(ILog log): base(log) { }

		public Task Handle(
			THandler handler, EventA @event, Func<Task> next, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var actualHandlerType = handler.GetType().GetFriendlyName();
			var eventType = @event.GetType().GetFriendlyName();
			Log($"{thisType}({actualHandlerType},{eventType})");
			return next();
		}
	}
}
