using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Events
{
	public class EventBHandler1: LoggingHandler, IEventHandler<EventB>
	{
		public EventBHandler1(ILog log): base(log) { }

		public Task Handle(EventB @event, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var eventType = @event.GetType().GetFriendlyName();
			Log($"{thisType}({eventType})");
			return Task.CompletedTask;
		}
	}
}
