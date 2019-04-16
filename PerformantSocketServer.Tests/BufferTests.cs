using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PerformantSocketServer.Tests
{
	[TestFixture]
	public class BufferTests
	{
		private const int TestPort = 20212;

		private void SendMessage(NetworkStream stream, string message)
		{
			var bytes = ASCIIEncoding.ASCII.GetBytes(message);

			stream.Write(bytes, 0, bytes.Length);
		}

		private void WaitForResponse(NetworkStream stream)
		{
			stream.ReadTimeout = 99999;

			var bytes = new byte[2000];
			var readLength = stream.Read(bytes, 0, bytes.Length);

			Assert.AreEqual("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: 0\r\n\r\n\r\n", Encoding.ASCII.GetString(bytes, 0, readLength));
		}

		private void WaitForNoResponse(NetworkStream stream)
		{
			stream.ReadTimeout = 99999;

			var bytes = new byte[2000];
			try
			{
				var readLength = stream.Read(bytes, 0, bytes.Length);

				Assert.AreEqual(0, readLength);
			}
			catch(IOException)
			{
				return;
			}
		}

		[Test]
		public void TestConnection()
		{
			var listenSettings = new SocketListenerSettings
			{
				LocalEndPoint = new IPEndPoint(IPAddress.Any, TestPort),
				MaxConnections = 10,
				MaxPendingConnections = 1000,
				MaxSimultaneousAcceptOperations = 2,
				IoBufferSize = 4096,
				ExcessConnectionHandlers = 5,
				ConnectionTimeOut = 100000,
				WatchDogCheckDelay = 10 * 1000, // 5 seconds
				MaxTaskConcurrencyLevel = 10,
				MaxTaskDelay = 3000,
			};

			var mockMessageHandler = new TestMessageHandler();
			
			using (var socketListener = new SocketListener(listenSettings, mockMessageHandler))
			{
				socketListener.StartListen();

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{

						SendMessage(stream,
							"GET /process.aspx?data=351777047213077|OGeNuj4NrSR00cY96P5E3Rc04h120Qd-000O001n0c1x HTTP/1.1\r\n" +
							"Host: 10.1.224.2\r\n" +
							"\r\n");

						WaitForResponse(stream);

						Assert.AreEqual(1, mockMessageHandler.HandledMessages);
					}
				}
			}
		}

		[Test]
		public void BufferOverflow()
		{
			var listenSettings = new SocketListenerSettings
			{
				LocalEndPoint = new IPEndPoint(IPAddress.Any, TestPort),
				MaxConnections = 10,
				MaxPendingConnections = 1000,
				MaxSimultaneousAcceptOperations = 2,
				IoBufferSize = 4096,
				ExcessConnectionHandlers = 5,
				ConnectionTimeOut = 100000,
				WatchDogCheckDelay = 10 * 1000,
				MaxTaskConcurrencyLevel = 10,
				MaxTaskDelay = 3000,
			};

			var mockMessageHandler = new TestMessageHandler();

			using (var socketListener = new SocketListener(listenSettings, mockMessageHandler))
			{
				socketListener.StartListen();

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						var gibberish = "";
						for (int i = 0; i < 4000; i++)
							gibberish += "bad";

						SendMessage(stream, gibberish);

						WaitForNoResponse(stream);

						Assert.AreEqual(0, mockMessageHandler.HandledMessages);
					}
				}

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream,
							"GET /process.aspx?data=351777047213077|OGeNuj4NrSR00cY96P5E3Rc04h120Qd-000O001n0c1x HTTP/1.1\r\n" +
							"Host: 10.1.224.2\r\n" +
							"\r\n");

						WaitForResponse(stream);

						Assert.AreEqual(1, mockMessageHandler.HandledMessages);
					}
				}
			}
		}

		[Test]
		public void ReusedBufferIsEmpty()
		{
			var listenSettings = new SocketListenerSettings
			{
				LocalEndPoint = new IPEndPoint(IPAddress.Any, TestPort),
				MaxConnections = 2,
				ExcessConnectionHandlers = 0,
				MaxPendingConnections = 1000,
				MaxSimultaneousAcceptOperations = 1,
				IoBufferSize = 4096,
				ConnectionTimeOut = 1000,
				WatchDogCheckDelay = 1000,
				MaxTaskConcurrencyLevel = 10,
				MaxTaskDelay = 3000,
			};

			var mockMessageHandler = new TestMessageHandler();

			using (var socketListener = new SocketListener(listenSettings, mockMessageHandler))
			{
				socketListener.StartListen();

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream,
							"GET /process.aspx?data=351777047213077|OGeNuj4NrSR00cY96P5E3Rc04h120Qd-000O001n0c1x HTTP/1.1\r\n" +
							"Host: 10.1.224.2\r\n" +
							"\r\n");

						WaitForResponse(stream);

						Assert.AreEqual(1, mockMessageHandler.HandledMessages);
					}
				}

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream, "");
						WaitForNoResponse(stream);

						Assert.AreEqual(1, mockMessageHandler.HandledMessages);
					}
				}

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream, "");
						WaitForNoResponse(stream);

						Assert.AreEqual(1, mockMessageHandler.HandledMessages);
					}
				}

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream,
							"GET /process.aspx?data=351777047213077|OGeNuj4NrSR00cY96P5E3Rc04h120Qd-000O001n0c1x HTTP/1.1\r\n" +
							"Host: 10.1.224.2\r\n" +
							"\r\n");

						WaitForResponse(stream);

						Assert.AreEqual(2, mockMessageHandler.HandledMessages);
					}
				}
			}
		}
	}
}
