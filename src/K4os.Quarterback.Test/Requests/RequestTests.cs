using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using K4os.Quarterback;
using Xunit;

namespace K4os.RoutR.Test.Requests
{
	public class RequestTests
	{
		private readonly Container _container = new Container();

		[Fact]
		public async Task FullySpecifiedRequestGetsExecuted()
		{
			Configure(typeof(RequestAHandler));
			var response = await _container
				.Expecting<ResponseA>()
				.Request(new RequestA());
			Assert.IsType<ResponseA>(response);
			Expect("RequestAHandler(RequestA)");
		}

		[Fact]
		public async Task HalfSpecifiedRequestGetsExecuted()
		{
			Configure(typeof(RequestAHandler));
			var response = await _container.Request(new RequestA());
			Assert.IsType<ResponseA>(response);
			Expect("RequestAHandler(RequestA)");
		}

		[Fact]
		public async Task DynamicallySpecifiedRequestGetsExecuted()
		{
			Configure(typeof(RequestAHandler));
			var response = await _container.RequestAny(new RequestA());
			Assert.IsType<ResponseA>(response);
			Expect("RequestAHandler(RequestA)");
		}

		[Theory]
		[InlineData(typeof(RequestA), typeof(ResponseA))]
		[InlineData(typeof(RequestB), typeof(ResponseB))]
		[InlineData(typeof(RequestC), typeof(ResponseC))]
		public async Task ClosestRequestIsPickedInMultiHandler(Type requestType, Type responseType)
		{
			Configure(typeof(RequestAbcHandler));
			var request = Activator.CreateInstance(requestType);
			var response = await _container.RequestAny(request);
			Assert.IsType(responseType, response);
			Expect($"RequestAbcHandler[{requestType.GetFriendlyName()}]({requestType.GetFriendlyName()})");
		}

		[Fact]
		public async Task PipelineIsExecutedWhenHandlingRequest()
		{
			Configure(
				typeof(RequestAbcHandler),
				typeof(GenericRequestAPipeline<,,>),
				typeof(ExplicitRequestBPipeline)
			);
			var request = new RequestB();
			var response = await _container.RequestAny(request);
			Assert.IsType<ResponseB>(response);
			Expect(
				"GenericRequestAPipeline<RequestAbcHandler,RequestB,ResponseB>(RequestAbcHandler,RequestB)",
				"ExplicitRequestBPipeline(RequestAbcHandler,RequestB)",
				"RequestAbcHandler[RequestB](RequestB)"
			);
		}

		private void Configure(params Type[] types)
		{
			_container.RegisterInstance<ILog>(new Log());
			_container.RegisterMany(types);
		}

		private void Expect(params string[] messages)
		{
			var snapshot = _container.Resolve<ILog>().Snapshot;
			Assert.Equal(messages, snapshot);
		}
	}
}
