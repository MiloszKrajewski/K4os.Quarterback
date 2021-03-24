using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace K4os.Quarterback
{
	/// <summary>
	/// Extension methods for ServiceProvider.
	/// </summary>
	public static class ProviderExtensions
	{
		/// <summary>Executed action in new scope.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="action">Action.</param>
		/// <returns>Result of action.</returns>
		public static void InNewScope(
			this IServiceProvider provider, Action<IServiceProvider> action)
		{
			using var scope = provider.CreateScope();
			action(scope.ServiceProvider);
		}

		/// <summary>Executed action in new scope.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="action">Action.</param>
		/// <returns>Result of action.</returns>
		public static T InNewScope<T>(
			this IServiceProvider provider, Func<IServiceProvider, T> action)
		{
			using var scope = provider.CreateScope();
			return action(scope.ServiceProvider);
		}

		/// <summary>Executed action in new scope.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="action">Action.</param>
		/// <returns>Result of action.</returns>
		public static async Task InNewScope(
			this IServiceProvider provider, Func<IServiceProvider, Task> action)
		{
			using var scope = provider.CreateScope();
			await action(scope.ServiceProvider);
		}

		/// <summary>Executed action in new scope.</summary>
		/// <param name="provider">Service provider.</param>
		/// <param name="action">Action.</param>
		/// <returns>Result of action.</returns>
		public static async Task<T> InNewScope<T>(
			this IServiceProvider provider, Func<IServiceProvider, Task<T>> action)
		{
			using var scope = provider.CreateScope();
			return await action(scope.ServiceProvider);
		}
	}
}
