using System.Threading;

namespace K4os.Quarterback.Test.Scopes
{
	public class ScopeCounter
	{
		private static int _counter;
		public int Instance { get; }
		public ScopeCounter() => Instance = Interlocked.Increment(ref _counter);
		public static void Reset() { _counter = 0; }
	}
}
