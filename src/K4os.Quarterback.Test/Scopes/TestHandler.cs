using System.Threading;
using System.Threading.Tasks;
using K4os.Quarterback.Abstractions;

namespace K4os.Quarterback.Test.Scopes
{
	public class TestHandler: ICommandHandler<TestCommand>
	{
		private readonly ScopeCounter _counter;

		public TestHandler(ScopeCounter counter) => _counter = counter;

		public Task Handle(TestCommand command, CancellationToken token)
		{
			command.Trace(_counter);
			return Task.CompletedTask;
		}
	}
}
