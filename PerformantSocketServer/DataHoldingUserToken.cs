using System;

namespace PerformantSocketServer
{
	public class DataHoldingUserToken : IdentityUserToken
	{
		public DataHoldingUserToken(int recieveBufferOffset, int writeBufferOffset, int bufferSize)
			: base()
		{
			ReceiveBufferOffset = recieveBufferOffset;
			WriteBufferOffset = writeBufferOffset;
			BufferSize = bufferSize;
		}

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

		internal void Reset()
		{
			ReceivedBytes = 0;
			WriteDataBytesSent = 0;
			WriteData = null;
		}
	}
}
