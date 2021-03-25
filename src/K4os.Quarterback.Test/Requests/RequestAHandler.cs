using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Requests
{
	public class RequestAHandler: LoggingHandler, IRequestHandler<RequestA, ResponseA>
	{
		public RequestAHandler(ILog log): base(log) { }

		public Task<ResponseA> Handle(RequestA request, CancellationToken token)
		{
			var thisType = this.GetType().GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}({requestType})");
			return Task.FromResult(new ResponseA());
		}
	}
}
