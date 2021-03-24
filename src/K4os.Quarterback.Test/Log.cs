using System.Collections.Generic;

namespace K4os.RoutR.Test
{
	public interface ILog
	{
		void Add(string message);
		string[] Snapshot { get; }
	}

	public class Log: ILog
	{
		private readonly List<string> _messages = new List<string>();

		public void Add(string message)
		{
			lock (_messages) _messages.Add(message);
		}

		public string[] Snapshot
		{
			get
			{
				lock (_messages) return _messages.ToArray();
			}
		}
	}
}
