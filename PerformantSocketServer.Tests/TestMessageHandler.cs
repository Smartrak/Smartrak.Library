using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformantSocketServer.Tests
{
	class TestMessageHandler  : IMessageHandler
	{
		public int HandledMessages { get; set; } = 0;

		public List<string> Messages { get; set; } = new List<string>();

		public bool IsMessageComplete(byte[] buffer, int startIdx, int length, int lengthNew)
		{
			return length >= 4 &&
					buffer[startIdx + length - 1] == '\n' &&
					buffer[startIdx + length - 2] == '\r' &&
					buffer[startIdx + length - 3] == '\n' &&
					buffer[startIdx + length - 4] == '\r';
		}

		public byte[] HandleMessage(byte[] buffer, int startIdx, int length)
		{
			HandledMessages ++;

			Messages.Add(Encoding.ASCII.GetString(buffer, startIdx, length));

			return Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: {0}\r\n\r\n{""}\r\n");
		}
	}
}
