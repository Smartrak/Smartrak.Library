
namespace PerformantSocketServer
{
	using System.Diagnostics;

	internal class ServerDiagnostics
	{
		private readonly Stopwatch _internalTime;
		private static readonly ServerDiagnostics _instance;

		private ServerDiagnostics()
		{
			_internalTime = new Stopwatch();
			_internalTime.Start();
		}

		static ServerDiagnostics()
		{
			_instance = Create();
		}

		public long Now
		{
			get { return _internalTime.ElapsedMilliseconds; }
		}

		public static ServerDiagnostics Instance
		{
			get { return _instance; }
		}

		private static ServerDiagnostics Create()
		{
			return new ServerDiagnostics();
		}
	}
}
