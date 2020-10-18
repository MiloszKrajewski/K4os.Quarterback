using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Queries
{
	public class QueryAbcHandler:
		LoggingHandler,
		IQueryHandler<QueryA, ResponseA>,
		IQueryHandler<QueryB, ResponseB>,
		IQueryHandler<QueryC, ResponseC>
	{
		public QueryAbcHandler(ILog log): base(log) { }

		public Task<ResponseA> Handle(QueryA query, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(QueryA).GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({queryType})");
			return Task.FromResult(new ResponseA());
		}

		public Task<ResponseB> Handle(
			QueryB query, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(QueryB).GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({queryType})");
			return Task.FromResult(new ResponseB());
		}

		public Task<ResponseC> Handle(
			QueryC query, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(QueryC).GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({queryType})");
			return Task.FromResult(new ResponseC());
		}
	}
}
