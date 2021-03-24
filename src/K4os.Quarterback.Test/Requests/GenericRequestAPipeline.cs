using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.RoutR.Test.Requests
{
	public class GenericRequestAPipeline<THandler, TRequest, TResponse>:
		LoggingHandler, IRequestPipeline<THandler, TRequest, TResponse>
		where THandler: IRequestHandler<TRequest, TResponse>
		where TRequest: RequestA
	{
		public GenericRequestAPipeline(ILog log): base(log) { }

		public Task<TResponse> Handle(
			THandler handler, TRequest request, Func<Task<TResponse>> next, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var handlerType = handler.GetType().GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}({handlerType},{requestType})");
			return next();
		}
	}
}
