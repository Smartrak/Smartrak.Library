using System;
using System.Net.Sockets;

namespace PerformantSocketServer
{
	public class DataHoldingUserToken<T> : IdentityUserToken where T : ISocketStateData
	{
		public DataHoldingUserToken(int recieveBufferOffset, int writeBufferOffset, int bufferSize)
			: base()
		{
			ReceiveBufferOffset = recieveBufferOffset;
			WriteBufferOffset = writeBufferOffset;
			BufferSize = bufferSize;

			//Create an instance of T
			SocketStateData = Activator.CreateInstance<T>();

			Id = Guid.NewGuid();
		}

		internal Guid Id { get; private set; }

		internal int ReceiveBufferOffset { get; private set; }
		internal int ReceivedBytes { get; set; }
		internal int ReceiveBufferNextPos
		{
			get { return ReceiveBufferOffset + ReceivedBytes; }
		}

		internal int WriteBufferOffset { get; private set; }
		internal byte[] WriteData { get; set; }
		internal int WriteDataBytesSent { get; set; }
		internal int WriteDataBytesRemaining
		{
			get { return WriteData.Length - WriteDataBytesSent; }
		}

		internal int BufferSize { get; private set; }

		internal bool CloseAfterSend { get; set; }
		internal bool ClosedByClient { get; set; }
		internal SocketError CloseReason { get; set; }

		internal T SocketStateData { get; private set; }

		internal void Reset()
		{
			ReceivedBytes = 0;
			WriteDataBytesSent = 0;
			WriteData = null;
			CloseAfterSend = false;
			ClosedByClient = false;
		}
	}
}
