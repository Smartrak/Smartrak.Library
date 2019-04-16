// Represents a collection of reusable SocketAsyncEventArgs objects.  
// https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx

namespace PerformantSocketServer
{
	using System;
	using System.Net.Sockets;
	using System.Collections.Generic;

	internal class SocketAsyncEventArgsPool
	{
		/// <summary>
		/// internal collection
		/// </summary>
		private readonly Stack<SocketAsyncEventArgs> _pool;

		/// <summary>
		/// max size of pool
		/// </summary>
		private readonly int _capacity;

		/// <summary>
		/// Initializes the object pool to the specified size
		/// </summary>
		/// <param name="capacity">Maximum number of SocketAsyncEventArgs the pool can hold</param>
		internal SocketAsyncEventArgsPool(int capacity)
		{
			_capacity = capacity;
			_pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		public int Count
		{
			get { return _pool.Count; }
		}

		public int Capacity
		{
			get { return _capacity; }
		}

		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			lock (_pool) _pool.Push(item);
		}

		public SocketAsyncEventArgs Pop()
		{
			lock (_pool) return _pool.Pop();
		}
	}
}
