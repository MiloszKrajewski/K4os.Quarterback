using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Requests
{
	public class ExplicitRequestBPipeline:
		LoggingHandler, IRequestPipeline<RequestAbcHandler, RequestB, ResponseB>
	{
		public ExplicitRequestBPipeline(ILog log): base(log) { }

		public Task<ResponseB> Handle(
			RequestAbcHandler handler, RequestB request, Func<Task<ResponseB>> next,
			CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var handlerType = handler.GetType().GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}({handlerType},{requestType})");
			return next();
		}
	}
}
