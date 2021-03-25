using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Mediator
{
	/// <summary>
	/// Default implementation for <see cref="IMediator"/>.
	/// Wraps around given <see cref="IServiceProvider"/>.
	/// It could be called Thomas as default quarterback but this reference
	/// could be a little bit obscure.
	/// </summary>
	public class QuarterbackMediator: IMediator
	{
		private readonly IServiceProvider _provider;

		/// <summary>
		/// Creates default implementation of <see cref="IMediator"/>.
		/// All dependencies will be resolved in passed scope. 
		/// </summary>
		/// <param name="provider"></param>
		public QuarterbackMediator(IServiceProvider provider) =>
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));

		/// <inheritdoc />
		public Task Send<TCommand>(TCommand command, CancellationToken token = default) =>
			_provider.Send(command, token);

		/// <inheritdoc />
		public Task SendAny(object command, CancellationToken token = default) =>
			_provider.SendAny(command, token);

		/// <inheritdoc />
		public Task Publish<TEvent>(TEvent @event, CancellationToken token = default) =>
			_provider.Publish(@event, token);

		/// <inheritdoc />
		public Task PublishAny(object @event, CancellationToken token = default) =>
			_provider.PublishAny(@event, token);

		/// <inheritdoc />
		public Task<TResponse> Request<TRequest, TResponse>(
			TRequest request, CancellationToken token = default) =>
			_provider.Request<TRequest, TResponse>(request, token);

		/// <inheritdoc />
		public Task<object> Request<TRequest>(
			TRequest request, CancellationToken token = default) =>
			_provider.Request(request, token);

		/// <inheritdoc />
		public Task<object> RequestAny(object request, CancellationToken token = default) =>
			_provider.RequestAny(request, token);

		/// <inheritdoc />
		public IRequestBuilder<TResponse> Expecting<TResponse>() =>
			_provider.Expecting<TResponse>();
	}
}
