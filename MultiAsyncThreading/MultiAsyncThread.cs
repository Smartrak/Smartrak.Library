using System;
using System.Threading;

namespace MultiAsyncThreading
{
	public class MultiAsyncThread : IAsyncCallbackRegister
	{
		private readonly Thread _thread;
		private readonly AsyncCallbackDispatcher _asyncCallbackDispatcher;

		public MultiAsyncThread()
		{
			_thread = new Thread(Run);
			_asyncCallbackDispatcher = new AsyncCallbackDispatcher(_thread);
		}

		public void Start()
		{
			_asyncCallbackDispatcher.Start();
			_thread.Start();
		}

		public bool Stop()
		{
			_asyncCallbackDispatcher.Stop();
			return _thread.Join(TimeSpan.FromSeconds(4));
		}

		private void Run()
		{
			_asyncCallbackDispatcher.Process();
		}

		public void Add(IAsyncResult asyncResult, Action<IAsyncResult> callback)
		{
			_asyncCallbackDispatcher.Add(asyncResult, callback);
		}
	}
}