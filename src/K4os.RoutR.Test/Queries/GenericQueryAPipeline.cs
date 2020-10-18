using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;

namespace K4os.RoutR.Test.Queries
{
	public class GenericQueryAPipeline<THandler, TQuery, TResponse>:
		LoggingHandler, IQueryPipeline<THandler, TQuery, TResponse>
		where THandler: IQueryHandler<TQuery, TResponse>
		where TQuery: QueryA
	{
		public GenericQueryAPipeline(ILog log): base(log) { }

		public Task<TResponse> Handle(
			THandler handler, TQuery query, Func<Task<TResponse>> next, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var handlerType = handler.GetType().GetFriendlyName();
			var queryType = query.GetType().GetFriendlyName();
			Log($"{thisType}({handlerType},{queryType})");
			return next();
		}
	}
}
