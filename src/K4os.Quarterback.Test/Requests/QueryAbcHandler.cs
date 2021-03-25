using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Requests
{
	public class RequestAbcHandler:
		LoggingHandler,
		IRequestHandler<RequestA, ResponseA>,
		IRequestHandler<RequestB, ResponseB>,
		IRequestHandler<RequestC, ResponseC>
	{
		public RequestAbcHandler(ILog log): base(log) { }

		public Task<ResponseA> Handle(RequestA request, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(RequestA).GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({requestType})");
			return Task.FromResult(new ResponseA());
		}

		public Task<ResponseB> Handle(
			RequestB request, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(RequestB).GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({requestType})");
			return Task.FromResult(new ResponseB());
		}

		public Task<ResponseC> Handle(
			RequestC request, CancellationToken token)
		{
			var thisType = GetType().GetFriendlyName();
			var declaredType = typeof(RequestC).GetFriendlyName();
			var requestType = request.GetType().GetFriendlyName();
			Log($"{thisType}[{declaredType}]({requestType})");
			return Task.FromResult(new ResponseC());
		}
	}
}
