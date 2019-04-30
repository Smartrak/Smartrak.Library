using System.Net;

namespace PerformantSocketServer
{
	public interface IMessageHandler<T> where T : ISocketStateData
	{
		/// <summary>
		/// Checks if recieved data represents a completed message
		/// </summary>
		/// <param name="buffer"> Shared message level buffer </param>
		/// <param name="startIdx"> start of message segment </param>
		/// <param name="length"> length of message segment </param>
		/// <param name="lengthNew"> length of message segment that is new for this completeness check (will be last N bytes)</param>
		/// <param name="socketStateData">Custom data that is passed around throughout the lifetime of the connection</param>
		/// <returns> True if message complete </returns>
		bool IsMessageComplete(byte[] buffer, int startIdx, int length, int lengthNew, T socketStateData);

		/// <summary>
		/// Process a message and return a response to be sent (or null)
		/// </summary>
		/// <param name="remoteEndPoint">The address of the endpoint when this method was called</param>
		/// <param name="buffer"> Shared message level buffer </param>
		/// <param name="startIdx"> start of message segment </param>
		/// <param name="length"> length of message segment </param>
		/// <param name="socketCustomState"> Custom state that the socket listener was created with </param>
		/// <returns> Resposne message (or null if no response is to be sent - connection will be closed) </returns>
		/// <param name="socketStateData">Custom data that is passed around throughout the lifetime of the connection</param>
		HandleMessageResponse HandleMessage(IPEndPoint remoteEndPoint, byte[] buffer, int startIdx, int length, object socketCustomState, T socketStateData);
	}

	public class HandleMessageResponse
	{
		public byte[] ToSend;
		public bool DisconnectOnceDone;
	}
}
