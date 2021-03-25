using System.Collections.Generic;
using System.Linq;

namespace K4os.Quarterback.Test.Scopes
{
	public class TestCommand
	{
		private readonly List<int> _result = new();
		public void Trace(ScopeCounter counter) => _result.Add(counter.Instance);
		public string Trace() => string.Join(",", _result.Select(v => v.ToString()));
	}
}
