using System;
using System.Linq;
using DryIoc;
using K4os.Quarterback;
using Xunit;

namespace K4os.RoutR.Test.Events
{
	public class EventTests
	{
		private readonly Container _container = new Container();

		[Fact]
		public void AllHandlersAreCalledWhenHandlingEvents()
		{
			Configure(typeof(EventAHandler1), typeof(EventAHandler2));
			var @event = new EventA();
			_container.Publish(@event);
			ExpectAll("EventAHandler1(EventA)", "EventAHandler2(EventA)");
		}

		[Fact]
		public void HandlerForDeclaredClassIsPreferred()
		{
			Configure(typeof(EventAHandler1), typeof(EventBHandler1));
			var @event = new EventB();
			_container.Publish<EventA>(@event);
			ExpectAll("EventAHandler1(EventB)");
		}

		[Fact]
		public void AllMatchingHandlersAreCalled()
		{
			Configure(typeof(EventAHandler1), typeof(EventBHandler1));
			var @event = new EventB();
			_container.Publish(@event);
			ExpectAll("EventBHandler1(EventB)", "EventAHandler1(EventB)");
		}

		[Fact]
		public void AllMatchingHandlersAreCalledWhenPublishingWithoutTypeSpecified()
		{
			Configure(typeof(EventAHandler1), typeof(EventBHandler1));
			var @event = new EventB();
			_container.PublishAny(@event);
			ExpectAll("EventBHandler1(EventB)", "EventAHandler1(EventB)");
		}

		[Fact]
		public void AllHandlersAreCalledInSeparatePipeline()
		{
			Configure(
				typeof(EventAHandler1),
				typeof(EventAHandler2),
				typeof(EventAAnyHandlerPipeline<>)
			);
			var @event = new EventA();
			_container.Publish(@event);

			ExpectAll(
				"EventAAnyHandlerPipeline<EventAHandler1>(EventAHandler1,EventA)",
				"EventAAnyHandlerPipeline<EventAHandler2>(EventAHandler2,EventA)",
				"EventAHandler1(EventA)",
				"EventAHandler2(EventA)"
			);
		}

		private void Configure(params Type[] types)
		{
			_container.RegisterInstance<ILog>(new Log());
			_container.RegisterMany(types);
		}

		private void ExpectAll(params string[] messages)
		{
			var snapshot = _container.Resolve<ILog>().Snapshot.OrderBy(x => x).ToArray();
			Assert.Equal(messages.OrderBy(x => x).ToArray(), snapshot);
		}
	}
}
