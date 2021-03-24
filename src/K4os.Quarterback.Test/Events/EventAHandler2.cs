using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.RoutR.Test.Events
{
	public class EventAHandler2: LoggingHandler, IEventHandler<EventA>
	{
		public EventAHandler2(ILog log): base(log) { }

		public Task Handle(EventA @event, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var eventType = @event.GetType().GetFriendlyName();
			Log($"{thisType}({eventType})");
			return Task.CompletedTask;
		}
	}
}
