namespace K4os.RoutR.Test
{
	public class LoggingHandler
	{
		private readonly ILog _log;

		public LoggingHandler(ILog log) { _log = log; }

		public void Log(string message) { _log.Add(message); }
	}
}
