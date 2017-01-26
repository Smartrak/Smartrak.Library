using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAsyncThreading
{
	public class AsyncCallbackDispatcher : IAsyncCallbackRegister
	{
		private ManualResetEvent _stopEvent;
		private readonly Thread _thread;
		private readonly Dictionary<WaitHandle, IAsyncResult> _asyncResults = new Dictionary<WaitHandle, IAsyncResult>();
		private readonly Dictionary<WaitHandle, Action<IAsyncResult>> _handleCallbacks = new Dictionary<WaitHandle, Action<IAsyncResult>>();
		private readonly List<WaitHandle> _waitHandles = new List<WaitHandle>();
		private bool _startedProcessing;

		public AsyncCallbackDispatcher(Thread thread)
		{
			_thread = thread;
		}

		public void Start()
		{
			_stopEvent = new ManualResetEvent(false);
			_waitHandles.Insert(0, _stopEvent);
			_startedProcessing = false;
		}

		public void Stop()
		{
			_stopEvent.Set();
			_thread.Join(TimeSpan.FromSeconds(4));
			_stopEvent.Dispose();
			_stopEvent = null;
		}

		public void Add(IAsyncResult asyncResult, Action<IAsyncResult> callback)
		{
			if (_startedProcessing && Thread.CurrentThread != _thread)
				throw new InvalidOperationException("Can only add new wait handles from the processing thread once processing has started");
			_asyncResults.Add(asyncResult.AsyncWaitHandle, asyncResult);
			_handleCallbacks.Add(asyncResult.AsyncWaitHandle, callback);
			_waitHandles.Add(asyncResult.AsyncWaitHandle);
		}

		public void Process()
		{
			if (Thread.CurrentThread != _thread)
				throw new InvalidOperationException("Can only call Process() from the processing thread");
			_startedProcessing = true;
			while (true)
			{
				int waitResult = WaitHandle.WaitAny(_waitHandles.ToArray());
				if (waitResult == 0)
					break;
				var waitHandle = _waitHandles[waitResult];
				var action = _handleCallbacks[waitHandle];
				var asyncResult = _asyncResults[waitHandle];
				_asyncResults.Remove(waitHandle);
				_handleCallbacks.Remove(waitHandle);
				_waitHandles.RemoveAt(waitResult);
				action(asyncResult);
			}
		}
	}
}