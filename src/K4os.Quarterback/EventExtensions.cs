using System;
using System.Threading;
using System.Threading.Tasks;
using EventHandler = K4os.Quarterback.Internals.EventHandler;

namespace K4os.Quarterback
{
	/// <summary>
	/// Extension methods to service provider to handle events.
	/// </summary>
	public static class EventExtensions
	{
		/// <summary>Publishes event to all registered compatible handlers.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="event">Event.</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <typeparam name="TEvent">Type of event (determines handlers).</typeparam>
		/// <returns>When handling is finished.</returns>
		public static Task Publish<TEvent>(
			this IServiceProvider provider, TEvent @event, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			@event.Required(nameof(@event));
			return EventHandler.Publish(provider, typeof(TEvent), @event!, token);
		}

		/// <summary>Publishes event to all registered compatible handlers.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="event">Event (its type determines handlers).</param>
		/// <param name="token"><see cref="CancellationToken"/></param>
		/// <returns>When handling is finished.</returns>
		public static Task PublishAny(
			this IServiceProvider provider, object @event, CancellationToken token = default)
		{
			provider.Required(nameof(provider));
			@event.Required(nameof(@event));
			return EventHandler.Publish(provider, @event.GetType(), @event, token);
		}
	}
}
