using System;
using System.Threading;
using System.Threading.Tasks;

namespace K4os.RoutR
{
	public static class EventExtensions
	{
		public static Task Publish<TEvent>(
			this IServiceProvider provider, TEvent @event, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			@event.Required(nameof(@event));
			return EventHandler.Publish(provider, typeof(TEvent), @event, token);
		}

		public static Task PublishAny(
			this IServiceProvider provider, object @event, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			@event.Required(nameof(@event));
			return EventHandler.Publish(provider, @event.GetType(), @event, token);
		}
	}
}
