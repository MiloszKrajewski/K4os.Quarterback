using System;

namespace K4os.Quarterback
{
	/// <summary>
	/// Default implementation for <see cref="IQuarterback"/>.
	/// Wraps around given <see cref="IServiceProvider"/>.
	/// It could be called Thomas as default quarterback but reference
	/// was a little bit obscure.
	/// </summary>
	public class Quarterback: IQuarterback
	{
		private readonly IServiceProvider _provider;

		/// <summary>
		/// Creates default implementation of <see cref="IQuarterback"/>.
		/// All dependencies will be resolved in passed scope. 
		/// </summary>
		/// <param name="provider"></param>
		public Quarterback(IServiceProvider provider) =>
			_provider = provider.Required(nameof(provider));

		/// <inheritdoc />
		public object GetService(Type serviceType) =>
			_provider.GetService(serviceType);
	}
}
