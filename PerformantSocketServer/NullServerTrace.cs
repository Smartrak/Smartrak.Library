using System;
using System.Net;
using System.Net.Sockets;

namespace PerformantSocketServer
{
	internal class NullServerTrace<TU>  : IServerTrace<TU> where TU : IListenerStateData
	{
		public void StartListen(SocketListenerSettings<TU> settings)
		{
		}

		public void DisposedListen(TU listenerStateObject, Exception err)
		{
		}

		public void HandleBadAccept(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote, SocketError error)
		{
		}

		public void HandleAccept(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote)
		{
		}

		public void Received(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length)
		{
		}

		public void ReceivedGreaterThanBuffer(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length)
		{
		}

		public void Sending(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote, byte[] toTransfer, int offset, int length)
		{
		}

		public void Sent(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, int bytesSent)
		{
		}

		public void ClosingConnection(TU listenerStateObject, IdentityUserToken connectionId, IPEndPoint remote, bool toldToCloseByHander, bool closedByClient, SocketError closeReason)
		{
		}

		public void TimingOutConnection(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length, double awaitTime, double obtainLock, double connectionsCollected, double closeTime)
		{
		}

		public void QueuedTask(TU listenerStateObject, IdentityUserToken poolId)
		{
		}

		public void StartingTask(TU listenerStateObject, IdentityUserToken poolId, DateTime requestedTime)
		{
		}

		public void ExpiredTask(TU listenerStateObject, IdentityUserToken poolId)
		{
		}

		public void CompletedTask(TU listenerStateObject, IdentityUserToken poolId)
		{
		}

		public void FailedTask(TU listenerStateObject, IdentityUserToken poolId, Exception e)
		{
		}

		public void WatchDogRunning(SocketListenerSettings<TU> settings, long time, double duration, int connectionCount)
		{
		}

		public void Exception(TU listenerStateObject, Exception err)
		{
		}

		public void Dispose(TU listenerStateObject)
		{
		}

		public void IncrementWatchedConnections(TU listenerStateObject)
		{
		
		}

		public void DecrementWatchedConnections(TU listenerStateObject)
		{
		}
	}
}
