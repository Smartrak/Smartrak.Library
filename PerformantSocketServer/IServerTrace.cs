using System;
using System.Net.Sockets;

namespace PerformantSocketServer
{
	using System.Net;

	public interface IServerTrace
	{
		void StartListen(SocketListenerSettings settings);
		void DisposedListen(Exception err);

		void HandleBadAccept(IdentityUserToken poolId, IPEndPoint remote, SocketError error);
		void HandleAccept(IdentityUserToken poolId, IPEndPoint remote);

		void Received(IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length);
		void ReceivedGreaterThanBuffer(IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length);

		void Sending(IdentityUserToken poolId, IPEndPoint remote, int bytesToTransfer);
		void Sent(IdentityUserToken poolId, IPEndPoint remote, int bytesSent);

		void ClosingConnection(IdentityUserToken poolId, IPEndPoint remote, bool toldToCloseByHander, bool closedByClient, SocketError closeReason);
		void TimingOutConnection(IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length, double awaitTime, double obtainLock, double connectionsCollected, double closeTime);

		void QueuedTask(IdentityUserToken poolId);
		void StartingTask(IdentityUserToken poolId, DateTime requestedTime);
		void ExpiredTask(IdentityUserToken poolId);
		void CompletedTask(IdentityUserToken poolId);
		void FailedTask(IdentityUserToken poolId, Exception e);

		void WatchDogRunning(long time, double duration);

		void Exception(Exception err);

		void Dispose();
		void IncrementWatchedConnections();
		void DecrementWatchedConnections();
	}
}
