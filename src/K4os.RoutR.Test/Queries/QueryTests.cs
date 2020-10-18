using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using Xunit;

namespace K4os.RoutR.Test.Queries
{
	public class QueryTests
	{
		private readonly Container _container = new Container();

		[Fact]
		public async Task FullySpecifiedQueryGetsExecuted()
		{
			Configure(typeof(QueryAHandler));
			var response = await _container
				.Expecting<ResponseA>()
				.Query(new QueryA());
			Assert.IsType<ResponseA>(response);
			Expect("QueryAHandler(QueryA)");
		}

		[Fact]
		public async Task HalfSpecifiedQueryGetsExecuted()
		{
			Configure(typeof(QueryAHandler));
			var response = await _container.Query(new QueryA());
			Assert.IsType<ResponseA>(response);
			Expect("QueryAHandler(QueryA)");
		}

		[Fact]
		public async Task DynamicallySpecifiedQueryGetsExecuted()
		{
			Configure(typeof(QueryAHandler));
			var response = await _container.QueryAny(new QueryA());
			Assert.IsType<ResponseA>(response);
			Expect("QueryAHandler(QueryA)");
		}

		[Theory]
		[InlineData(typeof(QueryA), typeof(ResponseA))]
		[InlineData(typeof(QueryB), typeof(ResponseB))]
		[InlineData(typeof(QueryC), typeof(ResponseC))]
		public async Task ClosestQueryIsPickedInMultiHandler(Type queryType, Type responseType)
		{
			Configure(typeof(QueryAbcHandler));
			var query = Activator.CreateInstance(queryType);
			var response = await _container.QueryAny(query);
			Assert.IsType(responseType, response);
			Expect(
				$"QueryAbcHandler[{queryType.GetFriendlyName()}]({queryType.GetFriendlyName()})");
		}

		[Fact]
		public async Task PipelineIsExecutedWhenHandlingQuery()
		{
			Configure(
				typeof(QueryAbcHandler),
				typeof(GenericQueryAPipeline<,,>),
				typeof(ExplicitQueryBPipeline)
			);
			var query = new QueryB();
			var response = await _container.QueryAny(query);
			Assert.IsType<ResponseB>(response);
			Expect(
				"GenericQueryAPipeline<QueryAbcHandler,QueryB,ResponseB>(QueryAbcHandler,QueryB)",
				"ExplicitQueryBPipeline(QueryAbcHandler,QueryB)",
				"QueryAbcHandler[QueryB](QueryB)"
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
