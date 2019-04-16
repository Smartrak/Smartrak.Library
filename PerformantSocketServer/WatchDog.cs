using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformantSocketServer
{
	/// <summary>
	/// Times out low running connections
	/// </summary>
	internal class WatchDog : IDisposable
	{
		private readonly HashSet<SocketAsyncEventArgs> _activeConnections;
		private readonly SocketListenerSettings _settings;
		private readonly CancellationTokenSource _cts;
		private readonly IServerTrace _trace;


		public WatchDog(SocketListenerSettings settings, IServerTrace trace)
		{
			_settings = settings;
			_trace = trace;

			_activeConnections = new HashSet<SocketAsyncEventArgs>();
			_cts = new CancellationTokenSource();
		}

		/// <summary>
		/// Starts thread to monitor watched connections
		/// </summary>
		public void Start()
		{
			Dog();
		}

		private async void Dog()
		{
			long startTime, awaitTime, lockObtainedTime, connectionsCollectedTime, beforeClosingTime;

			while (!_cts.Token.IsCancellationRequested)
			{
				startTime = ServerDiagnostics.Instance.Now;

				try
				{
					await Task.Delay(_settings.WatchDogCheckDelay, _cts.Token);
				}
				catch (TaskCanceledException)
				{
					// Handle being disposed gracefully.
				}

				awaitTime = ServerDiagnostics.Instance.Now;

				if (_cts.Token.IsCancellationRequested)
					return;

				List<SocketAsyncEventArgs> toClose = new List<SocketAsyncEventArgs>();
				lock (_activeConnections)
				{
					lockObtainedTime = ServerDiagnostics.Instance.Now;
					var currentTime = ServerDiagnostics.Instance.Now;

					foreach (var connection in _activeConnections)
					{
						var token = (IdentityUserToken)connection.UserToken;

						if (currentTime - token.LastTalked > _settings.ConnectionTimeOut)
						{
							// The connection has timed out, close it
							// the socket listener will handle the unexpected closure of
							// the connection like normal.
							toClose.Add(connection);
						}
					}
				}

				connectionsCollectedTime = ServerDiagnostics.Instance.Now;

				foreach (var v in toClose)
				{
					var token = (IdentityUserToken)v.UserToken;
					var dataToken = token as DataHoldingUserToken;

					// --- Logging / Debugging
					int bufferOffset = dataToken != null ? dataToken.ReceiveBufferOffset : 0;
					int receivedBytes = dataToken != null ? dataToken.ReceivedBytes : 0;
					var ipep = v.GetRemoteIpEndPoint();
					var buf = v.Buffer;
					// ----

					beforeClosingTime = ServerDiagnostics.Instance.Now;

					v.AcceptSocket.Close();

					_trace.TimingOutConnection(token, ipep, buf, bufferOffset, receivedBytes, awaitTime - startTime, lockObtainedTime - awaitTime, connectionsCollectedTime - lockObtainedTime, ServerDiagnostics.Instance.Now - beforeClosingTime);
				}

				// --- Logging / Debugging
				_trace.WatchDogRunning(ServerDiagnostics.Instance.Now, ServerDiagnostics.Instance.Now - startTime);
				// ---
			}
		}

		/// <summary>
		/// Ends monitor thread, times out all watched connections
		/// </summary>
		public void Dispose()
		{
			_cts.Cancel();

			lock (_activeConnections)
			{
				foreach (var connection in _activeConnections)
					connection.AcceptSocket.Close();

				_activeConnections.Clear();
			}
		}

		/// <summary>
		/// Begin watching a connection for timeouts
		/// </summary>
		/// <param name="eventArgs"></param>
		public void AddWatch(SocketAsyncEventArgs eventArgs)
		{
			lock (_activeConnections)
			{
				_activeConnections.Add(eventArgs);
			}
			_trace.IncrementWatchedConnections();
		}

		/// <summary>
		/// Stop watching a connection
		/// </summary>
		/// <param name="eventArgs"></param>
		public void RemoveWatch(SocketAsyncEventArgs eventArgs)
		{
			lock (_activeConnections)
			{
				_activeConnections.Remove(eventArgs);
			}
			_trace.DecrementWatchedConnections();
		}
	}
}
