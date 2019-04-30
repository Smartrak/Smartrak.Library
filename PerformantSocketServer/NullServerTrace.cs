using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PerformantSocketServer
{
	internal class NullServerTrace : IServerTrace
	{
		public void StartListen(SocketListenerSettings settings)
		{
		}

		public void DisposedListen(Exception err)
		{
		}

		public void HandleBadAccept(IdentityUserToken connectionId, IPEndPoint remote, SocketError error)
		{
		}

		public void HandleAccept(IdentityUserToken connectionId, IPEndPoint remote)
		{
		}

		public void Received(IdentityUserToken connectionId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length)
		{
		}

		public void ReceivedGreaterThanBuffer(IdentityUserToken connectionId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length)
		{
		}

		public void Sending(IdentityUserToken connectionId, IPEndPoint remote, int bytesTransferred)
		{
		}

		public void Sent(IdentityUserToken poolId, IPEndPoint remote, int bytesSent)
		{
		}

		public void ClosingConnection(IdentityUserToken connectionId, IPEndPoint remote, bool toldToCloseByHander, bool closedByClient, SocketError closeReason)
		{
		}

		public void TimingOutConnection(IdentityUserToken poolId, IPEndPoint remote, byte[] recieveBuffer, int startIdx, int length, double awaitTime, double obtainLock, double connectionsCollected, double closeTime)
		{
		}

		public void QueuedTask(IdentityUserToken poolId)
		{
		}

		public void StartingTask(IdentityUserToken poolId, DateTime requestedTime)
		{
		}

		public void ExpiredTask(IdentityUserToken poolId)
		{
		}

		public void CompletedTask(IdentityUserToken poolId)
		{
		}

		public void FailedTask(IdentityUserToken poolId, Exception e)
		{
		}

		public void WatchDogRunning(SocketListenerSettings settings, long time, double duration)
		{
		}

		public void Exception(Exception err)
		{
		}

		public void Dispose()
		{
		}

		public void IncrementWatchedConnections()
		{
		
		}

		public void DecrementWatchedConnections()
		{
		}
	}
}
