using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Queries
{
	public class QueryAHandler: LoggingHandler, IQueryHandler<QueryA, ResponseA>
	{
		public QueryAHandler(ILog log): base(log) { }

		public Task<ResponseA> Handle(QueryA query, CancellationToken token)
		{
			var thisType = this.GetType().GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}({queryType})");
			return Task.FromResult(new ResponseA());
		}
	}
}
