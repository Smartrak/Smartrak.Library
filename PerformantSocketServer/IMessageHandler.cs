namespace PerformantSocketServer
{
	public interface IMessageHandler
	{
		/// <summary>
		/// Checks if recieved data represents a completed message
		/// </summary>
		/// <param name="buffer"> Shared message level buffer </param>
		/// <param name="startIdx"> start of message segment </param>
		/// <param name="length"> length of message segment </param>
		/// <param name="lengthNew"> length of message segment that is new for this completeness check (will be last N bytes)</param>
		/// <returns> True if message complete </returns>
		bool IsMessageComplete(byte[] buffer, int startIdx, int length, int lengthNew);

		/// <summary>
		/// Process a message and return a response to be sent (or null)
		/// </summary>
		/// <param name="buffer"> Shared message level buffer </param>
		/// <param name="startIdx"> start of message segment </param>
		/// <param name="length"> length of message segment </param>
		/// <returns> Resposne message (or null if no response is to be sent - connection will be closed) </returns>
		byte[] HandleMessage(byte[] buffer, int startIdx, int length);
	}
}
