using System;
using System.Net.Sockets;

namespace PerformantSocketServer
{
	using System.Net;

	public interface IServerTrace<TU> where TU : IListenerStateData
	{
		void StartListen(SocketListenerSettings<TU> settings);
		void DisposedListen(TU listenerStateObject, Exception err);

		void HandleBadAccept(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, SocketError error);
		void HandleAccept(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote);

		void Received(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length);
		void ReceivedGreaterThanBuffer(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length);

		void Sending(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, int bytesToTransfer);
		void Sent(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, int bytesSent);

		void ClosingConnection(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, bool toldToCloseByHander, bool closedByClient, SocketError closeReason);
		void TimingOutConnection(TU listenerStateObject, IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length, double awaitTime, double obtainLock, double connectionsCollected, double closeTime);

		void QueuedTask(TU listenerStateObject, IdentityUserToken poolId);
		void StartingTask(TU listenerStateObject, IdentityUserToken poolId, DateTime requestedTime);
		void ExpiredTask(TU listenerStateObject, IdentityUserToken poolId);
		void CompletedTask(TU listenerStateObject, IdentityUserToken poolId);
		void FailedTask(TU listenerStateObject, IdentityUserToken poolId, Exception e);

		void WatchDogRunning(SocketListenerSettings<TU> settings, long time, double duration, int connectionCount);

		void Exception(TU listenerStateObject, Exception err);

		void Dispose(TU listenerStateObject);
		void IncrementWatchedConnections(TU listenerStateObject);
		void DecrementWatchedConnections(TU listenerStateObject);
	}
}
