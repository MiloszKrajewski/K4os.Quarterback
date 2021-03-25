using System;
using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Scopes
{
	public class TestPipeline: ICommandPipeline<TestHandler, TestCommand>
	{
		private readonly ScopeCounter _counter;

		public TestPipeline(ScopeCounter counter) => _counter = counter;

		public async Task Handle(
			TestHandler handler, TestCommand command, Func<Task> next, CancellationToken token)
		{
			command.Trace(_counter);
			await next();
			command.Trace(_counter);
		}
	}
}
