// This class creates a single large buffer which can be divided up 
// and assigned to SocketAsyncEventArgs objects for use with each 
// socket I/O operation.  
// This enables buffers to be easily reused and guards against 
// fragmenting heap memory.
// 
// This buffer is a byte array which the Windows TCP buffer can copy its data to.
// https://msdn.microsoft.com/en-us/library/bb517542.aspx

namespace PerformantSocketServer
{
	using System.Collections.Generic;
	using System.Net.Sockets;

	internal class BufferManager
	{
		/// <summary>
		/// The total number of bytes controlled
		/// </summary>
		protected int BufferSize;

		/// <summary>
		/// The internal buffer
		/// </summary>
		protected byte[] Buffer;

		/// <summary>
		/// amount of bytes each item takes in the buffer
		/// </summary>
		protected int ItemSize;

		/// <summary>
		/// Manages avaliable locations in buffer
		/// </summary>
		protected Stack<int> FreeIndexPool; 
		protected int CurrentIndex;
		

		public BufferManager(int totalBytes, int bytesPerObject)
		{
			BufferSize = totalBytes;
			CurrentIndex = 0;
			ItemSize = bytesPerObject;
			FreeIndexPool = new Stack<int>();
		}

		// Allocates buffer space used by the buffer pool
		public void InitBuffer()
		{
			// create one big large buffer and divide that 
			// out to each SocketAsyncEventArg object
			Buffer = new byte[BufferSize];
		}

		// Assigns a buffer from the buffer pool to the 
		// specified SocketAsyncEventArgs object
		//
		// <returns>true if the buffer was successfully set, else false</returns>
		public bool SetBuffer(SocketAsyncEventArgs args)
		{
			if (FreeIndexPool.Count > 0)
			{
				// Use a previously freed space if avaliable
				args.SetBuffer(Buffer, FreeIndexPool.Pop(), ItemSize);
			}
			else
			{
				// Otherwise ensure enough space in buffer
				if ((BufferSize - ItemSize) < CurrentIndex)
					return false;

				args.SetBuffer(Buffer, CurrentIndex, ItemSize);
				CurrentIndex += ItemSize;
			}

			return true;
		}

		// Removes the buffer from a SocketAsyncEventArg object.  
		// This frees the buffer back to the buffer pool
		public void FreeBuffer(SocketAsyncEventArgs args)
		{
			FreeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

	}

}
