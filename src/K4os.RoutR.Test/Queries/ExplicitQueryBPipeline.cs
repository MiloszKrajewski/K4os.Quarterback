using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Queries
{
	public class ExplicitQueryBPipeline:
		LoggingHandler, IQueryPipeline<QueryAbcHandler, QueryB, ResponseB>
	{
		public ExplicitQueryBPipeline(ILog log): base(log) { }

		public Task<ResponseB> Handle(
			QueryAbcHandler handler, QueryB query, Func<Task<ResponseB>> next,
			CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var handlerType = handler.GetType().GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}({handlerType},{queryType})");
			return next();
		}
	}
}
