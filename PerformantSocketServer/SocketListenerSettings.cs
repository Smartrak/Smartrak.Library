namespace PerformantSocketServer
{
	using System.Net;

	public class SocketListenerSettings
	{
		/// <summary>
		/// The maximum number of connections to handle simultaneously 
		/// </summary>
		public int MaxConnections { get; set; }

		/// <summary>
		/// The maximum number of connections the listener can hold queued
		/// </summary>
		public int MaxPendingConnections { get; set; }

		/// <summary>
		/// The maximum number of asynchronous accept operations that can be 
		/// handled simultaneously. This determines the size of the pool of 
		/// SocketAsyncEventArgs objects that do accept operations.
		/// </summary>
		public int MaxSimultaneousAcceptOperations { get; set; }

		/// <summary>
		/// Buffer size to use for each socket recieve / write operation
		/// Must be large enough to recieve full message from client. (Todo: grow recieve buffer?)
		/// Written messages may be larger than the buffer.
		/// </summary>
		public int IoBufferSize { get; set; }

		/// <summary>
		/// Socket to listen on
		/// </summary>
		public IPEndPoint LocalEndPoint { get; set; }

		/// <summary>
		/// Allows us to create some extra SocketAsyncEventArgs objects for the send/recieve pool
		/// </summary>
		public int ExcessConnectionHandlers { get; set; }

		/// <summary>
		/// How many milliseconds the watchdog should give a conection to talk before killing it.
		/// </summary>
		public long ConnectionTimeOut { get; set; }

		/// <summary>
		/// The amount of milliseconds the watch dog should wait between each timeout check 
		/// </summary>
		public int WatchDogCheckDelay { get; set; }

		/// <summary>
		/// The amount of tasks which can be handled at a time
		/// </summary>
		public int MaxTaskConcurrencyLevel { get; set; }

		/// <summary>
		/// The amount of seconds a task can be delayed for before it is canceled.
		/// </summary>
		public int MaxTaskDelay { get; set; }

		/// <summary>
		/// Total number of SocketAsyncEventArgs in the pool for sending/recieving
		/// </summary>
		public int SendReceiveSocketPoolSize
		{
			get { return MaxConnections + ExcessConnectionHandlers; }
		}

		/// <summary>
		/// Send/Recieve
		/// </summary>
		public int OperationTypesToPreAllocate
		{
			get { return 2; }
		}

		/// <summary>
		/// A custom supplied object from whoever constructs this to pass down into the Socket Listener.
		/// </summary>
		public object CustomState { get; set; }
	}
}
