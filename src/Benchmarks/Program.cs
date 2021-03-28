using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
			// ProfileQuarterback().Wait();
		}

		// ReSharper disable once UnusedMember.Local
		private static async Task ProfileQuarterback()
		{
			var sut = new Commands();
			sut.Setup();
			var cts = new CancellationTokenSource(30 * 1000);
			while (!cts.Token.IsCancellationRequested)
				for (var i = 0; i < 1000; i++)
					await sut.UseStaticQuarterback();
		}
	}
}
