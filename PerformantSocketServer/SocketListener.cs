// See https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx

using System.Net;
using System.Threading.Tasks;
using LiteScriptsService;

namespace PerformantSocketServer
{
	using System;
	using System.Net.Sockets;
	using System.Threading;

	public class SocketListener<T> : IDisposable where T : ISocketStateData
	{
		/// <summary>
		/// Server settings
		/// </summary>
		private readonly SocketListenerSettings _settings;

		/// <summary>
		/// Provides method to check if a message has been completely recieved
		/// And a method to handle the message / create a response
		/// (Bussiness logic about the message goes in here)
		/// </summary>
		private readonly IMessageHandler<T> _messageHandler;

		/// <summary>
		/// Allows for callbacks to be called at different stages throughout server run.
		/// </summary>
		private readonly IServerTrace _trace;

		/// <summary>
		/// Socket used to listen for incoming connection requests
		/// </summary>
		private Socket _listenSocket;

		/// <summary>
		/// Buffers for sockets are unmanaged by .NET. 
		/// So memory used for buffers gets "pinned", which makes the
		/// .NET garbage collector work around it, fragmenting the memory. 
		/// Circumvent this problem by putting all buffers together 
		/// in one block in memory. Then we will assign a part of that space 
		/// to each SocketAsyncEventArgs object, and
		/// reuse that buffer space each time we reuse the SocketAsyncEventArgs object.
		/// Create a large reusable set of buffers for all socket operations.
		/// </summary>
		private readonly BufferManager _bufferManager;

		/// <summary>
		/// Pool of reusable SocketAsyncEventArgs used for Receive/send socket operations
		/// </summary>
		private readonly SocketAsyncEventArgsPool _workerPool;

		/// <summary>
		/// Pool of reusable SocketAsyncEventArgs used for accept operations
		/// We use a seperate pool for accept operations as they don't need a buffer.
		/// </summary>
		private readonly SocketAsyncEventArgsPool _acceptPool;

		/// <summary>
		/// Monitors active connections and timesout connections that have not talked
		/// within the last N milliseconds. Will kill active connections when disposed.
		/// </summary>
		private readonly WatchDog<T> _watchDog;

		/// <summary>
		/// This Semaphore is used to keep from going over the maximum connection number.
		/// It is not about controlling threading here really.
		/// </summary>
		private readonly Semaphore _maxConnectionsEnforcer;

		/// <summary>
		/// Handles running requests with a limited amount running in parrallel.
		/// Queues requests if needed.
		/// </summary>
		private readonly TaskFactory _taskManager;

		public SocketListener(SocketListenerSettings settings, IMessageHandler<T> messageHandler, IServerTrace trace = null)
		{
			_settings = settings;
			_messageHandler = messageHandler;
			_trace = trace ?? new NullServerTrace();

			// Create buffer
			var totalBufferSize = settings.IoBufferSize * settings.SendReceiveSocketPoolSize * settings.OperationTypesToPreAllocate;
			var itemBufferSize = settings.IoBufferSize * settings.OperationTypesToPreAllocate;
			_bufferManager = new BufferManager(totalBufferSize, itemBufferSize);

			// Create socket pool
			_workerPool = new SocketAsyncEventArgsPool(settings.SendReceiveSocketPoolSize);
			_acceptPool = new SocketAsyncEventArgsPool(settings.MaxSimultaneousAcceptOperations);

			// Create watchdog
			_watchDog = new WatchDog<T>(_settings, _trace);

			// Create connection count enforcer
			_maxConnectionsEnforcer = new Semaphore(settings.MaxConnections, settings.MaxConnections);

			_taskManager = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(settings.MaxTaskConcurrencyLevel));

			Initialise();
		}

		/// <summary>
		/// Initializes the server by preallocating reusable buffers and SocketAsyncEventArgs objects
		/// </summary>
		private void Initialise()
		{
			_bufferManager.InitBuffer();

			for (var i = 0; i < _acceptPool.Capacity; i++)
				_acceptPool.Push(CreateAcceptingSocketAsyncEventArgs());

			for (var i = 0; i < _workerPool.Capacity; i++)
				_workerPool.Push(CreateWorkerSocketAsyncEventArgs());
		}

		/// <summary>
		/// Begin listening for incoming connections
		/// </summary>
		public void StartListen()
		{
			// --- Logging / Debugging
			_trace.StartListen(_settings);
			// ---

			_watchDog.Start();

			_listenSocket = new Socket(_settings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(_settings.LocalEndPoint);
			_listenSocket.Listen(_settings.MaxPendingConnections);

			StartAccept();
		}

		/// <summary>
		/// Begins an operation to accept a connection from a client
		/// </summary>
		private void StartAccept()
		{
			SocketAsyncEventArgs acceptEventArg;

			if (_acceptPool.Count > 1)
			{
				try
				{
					acceptEventArg = _acceptPool.Pop();
				}
				catch
				{
					acceptEventArg = CreateAcceptingSocketAsyncEventArgs();
				}
			}
			else
			{
				acceptEventArg = CreateAcceptingSocketAsyncEventArgs();
			}


			// This is a mechanism to prevent exceeding
			// the max # of connections we specified. We'll do this before
			// doing AcceptAsync. If maxConnections value has been reached,
			// then the application will pause here until the Semaphore gets released,
			// which happens in the CloseClientSocket method.        
			_maxConnectionsEnforcer.WaitOne();

			try
			{
				bool runningAsync = _listenSocket.AcceptAsync(acceptEventArg);

				if (!runningAsync)
				{
					// Accept completed synchronously (Completed callback was not called).
					ProcessAccept(acceptEventArg);
				}
			}
			catch (ObjectDisposedException err)
			{
				// Handle silently, the program is most likely being terminated.
				// Trace just in case we get here in an unexpected way.

				_trace.DisposedListen(err);
			}
		}

		/// <summary>
		/// Setup connection to use a SocketAsyncEventArgs setup to send/Receive
		/// </summary>
		/// <param name="acceptEventArgs"></param>
		private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
		{
			IdentityUserToken token = (IdentityUserToken)acceptEventArgs.UserToken;
			token.LastTalked = ServerDiagnostics.Instance.Now;

			// Handle failure to accept connection
			if (acceptEventArgs.SocketError != SocketError.Success)
			{
				StartAccept();

				HandleBadAccept(acceptEventArgs, acceptEventArgs.SocketError);

				return;
			}

			_trace.HandleAccept(token, acceptEventArgs.GetRemoteIpEndPoint());

			// Begin listening for more connections
			StartAccept();

			var workerEventArgs = GetWorkerFromAcceptEventArgs(acceptEventArgs);
			_watchDog.AddWatch(workerEventArgs);

			StartReceive(workerEventArgs);
		}

		private SocketAsyncEventArgs GetWorkerFromAcceptEventArgs(SocketAsyncEventArgs acceptEventArgs)
		{
			var workerEventArgs = _workerPool.Pop();
			workerEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;

			// Ensure that the buffer we're about to use has been cleared.
			((DataHoldingUserToken<T>)workerEventArgs.UserToken).LastTalked = ((IdentityUserToken)acceptEventArgs.UserToken).LastTalked;
			((DataHoldingUserToken<T>)workerEventArgs.UserToken).Reset();
			((DataHoldingUserToken<T>)workerEventArgs.UserToken).SocketStateData.Reset();

			acceptEventArgs.AcceptSocket = null;
			_acceptPool.Push(acceptEventArgs);

			return workerEventArgs;
		}

		private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs, SocketError error)
		{
			// --- Logging / Debugging
			_trace.HandleBadAccept((IdentityUserToken)acceptEventArgs.UserToken, acceptEventArgs.GetRemoteIpEndPoint(), error);
			// ---

			acceptEventArgs.AcceptSocket.Close();
			acceptEventArgs.AcceptSocket = null;
			_acceptPool.Push(acceptEventArgs);
		}

		/// <summary>
		/// Begin listening for data on accepted connection
		/// </summary>
		/// <param name="workerEventArgs"></param>
		private void StartReceive(SocketAsyncEventArgs workerEventArgs)
		{
			bool runningAsync;

			try
			{
				var token = (DataHoldingUserToken<T>)workerEventArgs.UserToken;

				workerEventArgs.SetBuffer(token.ReceiveBufferNextPos, _settings.IoBufferSize);

				runningAsync = workerEventArgs.AcceptSocket.ReceiveAsync(workerEventArgs);
			}
			catch (Exception err)
			{
				_trace.Exception(err);

				// Close the socket so that the process receive 
				// will receive a socket closed event and clean up.

				workerEventArgs.AcceptSocket.Close();
				return;
			}

			if (!runningAsync)
			{
				ProcessReceive(workerEventArgs);
			}
		}

		private void ProcessReceive(SocketAsyncEventArgs workerEventArgs)
		{
			var token = (DataHoldingUserToken<T>)workerEventArgs.UserToken;
			token.ReceivedBytes += workerEventArgs.BytesTransferred;
			token.LastTalked = ServerDiagnostics.Instance.Now;

			// Check the socket is still good
			bool closeSocket =
				workerEventArgs.BytesTransferred == 0 || // Client has closed the connection (Normal)
				workerEventArgs.SocketError != SocketError.Success || // Something has gone wrong. (Bad)
				token.ReceivedBytes > token.BufferSize;  // Recieved more data then we can handle (Bad)

			// if not close the socket
			if (closeSocket)
			{
				if (token.ReceivedBytes > token.BufferSize && workerEventArgs.SocketError == SocketError.Success)
					_trace.ReceivedGreaterThanBuffer(token, workerEventArgs.GetRemoteIpEndPoint(), workerEventArgs.Buffer, token.ReceiveBufferOffset, token.ReceivedBytes);

				//token.Reset();
				token.ClosedByClient = workerEventArgs.BytesTransferred == 0;
				token.CloseReason = workerEventArgs.SocketError;
				CloseClientSocket(workerEventArgs);

				return;
			}

			//Create a clone of the endpoint, as having a task might mean accessing the socket details after the endpoint is no longer valid.
			var currentEndpoint = workerEventArgs.GetRemoteIpEndPoint();
			IPEndPoint ipEndPoint = new IPEndPoint(currentEndpoint.Address, currentEndpoint.Port);

			// --- Logging / Debugging
			_trace.Received(token, ipEndPoint, workerEventArgs.Buffer, token.ReceiveBufferOffset, token.ReceivedBytes);
			// ---

			// Check if we have a full message
			bool incomingMessageReady =
				_messageHandler.IsMessageComplete(workerEventArgs.Buffer, token.ReceiveBufferOffset, token.ReceivedBytes, workerEventArgs.BytesTransferred, token.SocketStateData);

			if (incomingMessageReady)
			{
				// If so handle it, and send a response
				// if we have one

				// --- Logging / Debugging
				_trace.QueuedTask(token);
				// ---

				var requestStartTime = DateTime.Now;

				_taskManager.StartNew(state =>
				{
					SocketListenerState s = (SocketListenerState) state;
					try
					{
						// --- Logging / Debugging
						_trace.StartingTask(token, requestStartTime);
						// ---

						if ((DateTime.Now - requestStartTime).TotalSeconds > _settings.MaxTaskDelay)
						{
							_trace.ExpiredTask(token);
							CloseClientSocket(workerEventArgs);

							return;
						}

						var response = _messageHandler.HandleMessage(s.EndPoint, workerEventArgs.Buffer, token.ReceiveBufferOffset, token.ReceivedBytes, _settings.CustomState, token.SocketStateData);
						token.Reset();
						token.WriteData = response.ToSend;
						token.CloseAfterSend = response.DisconnectOnceDone;

						if (token.WriteData != null)
							StartSend(workerEventArgs);
						else if (response.DisconnectOnceDone)
							CloseClientSocket(workerEventArgs);
						else
							StartReceive(workerEventArgs);

						// --- Logging / Debugging
						_trace.CompletedTask(token);
						// ---

					}
					catch (Exception e)
					{
						// Task failed...
						_trace.FailedTask(token, e);
						CloseClientSocket(workerEventArgs);
					}
				}, new SocketListenerState { EndPoint = ipEndPoint });
			}
			else
			{
				// otherwise wait for more data to be recieved
				StartReceive(workerEventArgs);
			}
		}

		/// <summary>
		/// Begin sending data
		/// </summary>
		/// <param name="workerEventArgs"></param>
		private void StartSend(SocketAsyncEventArgs workerEventArgs)
		{
			DataHoldingUserToken<T> token = (DataHoldingUserToken<T>)workerEventArgs.UserToken;

			if (token.WriteDataBytesRemaining <= _settings.IoBufferSize)
			{
				// Whole remaining message will fit in buffer.
				workerEventArgs.SetBuffer(token.WriteBufferOffset, token.WriteDataBytesRemaining);
				Buffer.BlockCopy(token.WriteData, token.WriteDataBytesSent, workerEventArgs.Buffer, token.WriteBufferOffset, token.WriteDataBytesRemaining);

				// --- Logging / Debugging
				_trace.Sending(token, workerEventArgs.GetRemoteIpEndPoint(), token.WriteDataBytesRemaining);
				// ---
			}
			else
			{
				// Message to big to all go in buffer.

				workerEventArgs.SetBuffer(token.WriteBufferOffset, _settings.IoBufferSize);
				Buffer.BlockCopy(token.WriteData, token.WriteDataBytesSent, workerEventArgs.Buffer, token.WriteBufferOffset, _settings.IoBufferSize);

				// We update the token.WriteDataBytesSent once the Sending has been confirmed.

				// --- Logging / Debugging
				_trace.Sending(token, workerEventArgs.GetRemoteIpEndPoint(), _settings.IoBufferSize);
				// ---
			}


			bool runningAsync = workerEventArgs.AcceptSocket.SendAsync(workerEventArgs);

			if (!runningAsync)
			{
				ProcessSend(workerEventArgs);
			}
		}

		private void ProcessSend(SocketAsyncEventArgs workerEventArgs)
		{
			DataHoldingUserToken<T> token = (DataHoldingUserToken<T>)workerEventArgs.UserToken;
			token.WriteDataBytesSent += workerEventArgs.BytesTransferred;

			// --- Logging / Debugging
			_trace.Sent(token, workerEventArgs.GetRemoteIpEndPoint(), workerEventArgs.BytesTransferred);
			// ---

			if (workerEventArgs.SocketError != SocketError.Success)
			{
				token.Reset();
				CloseClientSocket(workerEventArgs);

				return;
			}

			if (token.WriteDataBytesRemaining > 0)
			{
				// If some of the bytes in the message have NOT been sent,
				// then we will need to post another send operation
				StartSend(workerEventArgs);
			}
			else
			{
				if (token.CloseAfterSend)
					CloseClientSocket(workerEventArgs);
				else
					StartReceive(workerEventArgs);
			}
		}

		private void CloseClientSocket(SocketAsyncEventArgs workerEventArgs)
		{
			var token = (DataHoldingUserToken<T>)workerEventArgs.UserToken;

			// --- Logging / Debugging
			_trace.ClosingConnection(token, workerEventArgs.GetRemoteIpEndPoint(), token.CloseAfterSend, token.ClosedByClient, token.CloseReason);
			// ---

			_watchDog.RemoveWatch(workerEventArgs);

			// do a shutdown before you close the socket
			try
			{
				workerEventArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
			}
			// throws if socket was already closed
			catch { }

			//This method closes the socket and releases all resources, both
			//managed and unmanaged. It internally calls Dispose.
			workerEventArgs.AcceptSocket.Close();
			token.Reset();

			_workerPool.Push(workerEventArgs);

			//Release Semaphore so that its connection counter will be decremented.
			//This must be done AFTER putting the SocketAsyncEventArg back into the pool,
			//or you can run into problems.
			_maxConnectionsEnforcer.Release();
		}

		/// <summary>
		/// Creates a new SocketAsyncEventArgs object to do accept operations
		/// </summary>
		/// <returns></returns>
		private SocketAsyncEventArgs CreateAcceptingSocketAsyncEventArgs()
		{
			SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
			acceptEventArg.Completed += AcceptOnCompleted;
			acceptEventArg.UserToken = new IdentityUserToken();

			return acceptEventArg;
		}

		/// <summary>
		/// Creates a new SocketAsyncEventArgs object to do send/Receive operations
		/// </summary>
		/// <returns></returns>
		private SocketAsyncEventArgs CreateWorkerSocketAsyncEventArgs()
		{
			SocketAsyncEventArgs workerEventArgs = new SocketAsyncEventArgs();
			workerEventArgs.Completed += WorkerIoOnCompleted;

			_bufferManager.SetBuffer(workerEventArgs);
			workerEventArgs.UserToken = new DataHoldingUserToken<T>(workerEventArgs.Offset, workerEventArgs.Offset + _settings.IoBufferSize, _settings.IoBufferSize);

			return workerEventArgs;
		}

		private void WorkerIoOnCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
		{
			switch (socketAsyncEventArgs.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					ProcessReceive(socketAsyncEventArgs);
					break;

				case SocketAsyncOperation.Send:
					ProcessSend(socketAsyncEventArgs);
					break;

				default:
					throw new NotImplementedException("The last opreation completed on the socket was not a receive or send");
			}
		}

		private void AcceptOnCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
		{
			ProcessAccept(socketAsyncEventArgs);
		}

		public void Dispose()
		{
			// --- Logging / Debugging
			_trace.Dispose();
			// ---

			_listenSocket?.Close();
			_watchDog.Dispose();

			SocketAsyncEventArgs eventArgs;
			while (_acceptPool.Count > 0)
			{
				eventArgs = _acceptPool.Pop();
				eventArgs.Dispose();
			}
			while (_workerPool.Count > 0)
			{
				eventArgs = _workerPool.Pop();
				eventArgs.Dispose();
			}
		}
	}

	internal static class SocketAsyncEventArgsExtensions
	{
		public static IPEndPoint GetRemoteIpEndPoint(this SocketAsyncEventArgs eventArgs)
		{
			try
			{
				return eventArgs.AcceptSocket != null ? eventArgs.AcceptSocket.RemoteEndPoint as IPEndPoint : null;
			}
			catch (ObjectDisposedException)
			{
				return null;
			}
		}
	}

	public class SocketListenerState
	{
		public IPEndPoint EndPoint { get; set; }
	}

}
