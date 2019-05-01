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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace PerformantSocketServer.Tests
{
	[TestFixture]
	public class BufferTests
	{
		[SetUp]
		public void Setup()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new IPAddressConverter());
			settings.Converters.Add(new IPEndPointConverter());
			settings.Formatting = Formatting.Indented;

			JsonConvert.DefaultSettings = () => settings;
		}

		private const int TestPort = 20212;

		private void SendMessage(NetworkStream stream, string message)
		{
			var bytes = ASCIIEncoding.ASCII.GetBytes(message);

			stream.Write(bytes, 0, bytes.Length);
		}

		private void WaitForResponse(NetworkStream stream)
		{
			stream.ReadTimeout = 999;

			var bytes = new byte[2000];
			var readLength = stream.Read(bytes, 0, bytes.Length);

			Assert.AreEqual("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: 0\r\n\r\n\r\n", Encoding.ASCII.GetString(bytes, 0, readLength));
		}

		private void WaitForNoResponse(NetworkStream stream)
		{
			stream.ReadTimeout = 999;

			var bytes = new byte[2000];
			try
			{
				var readLength = stream.Read(bytes, 0, bytes.Length);

				Assert.AreEqual(0, readLength);
			}
			catch (IOException)
			{
				return;
			}
		}

		[Test]
		public void TestConnection()
		{
			var listenSettings = new SocketListenerSettings<IListenerStateData>
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

			var mockMessageHandler = new TestMessageHandler<TestingSocketStateData, IListenerStateData>();

			using (var socketListener = new SocketListener<TestingSocketStateData, IListenerStateData>(listenSettings, mockMessageHandler))
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
			var listenSettings = new SocketListenerSettings<IListenerStateData>
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

			var mockMessageHandler = new TestMessageHandler<TestingSocketStateData, IListenerStateData>();
			using (var socketListener = new SocketListener<TestingSocketStateData, IListenerStateData>(listenSettings, mockMessageHandler))
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

						WaitForNoResponse(stream);
					}
				}
			}
		}

		[Test]
		public void ReusedBufferIsEmpty()
		{
			var listenSettings = new SocketListenerSettings<IListenerStateData>
			{
				LocalEndPoint = new IPEndPoint(IPAddress.Any, TestPort),
				MaxConnections = 2,
				ExcessConnectionHandlers = 10,
				MaxPendingConnections = 1000,
				MaxSimultaneousAcceptOperations = 1,
				IoBufferSize = 4096,
				ConnectionTimeOut = 5000,
				WatchDogCheckDelay = 2000,
				MaxTaskConcurrencyLevel = 10,
				MaxTaskDelay = 3000,
			};
			const double ticksPerSecond = 10000000.0;
			var mockMessageHandler = new TestMessageHandler<TestingSocketStateData, IListenerStateData>();
			Mock<IServerTrace<IListenerStateData>> tracer = new Mock<IServerTrace<IListenerStateData>>();
			tracer.Setup(x => x.Sending(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
				.Callback<IListenerStateData, IdentityUserToken, IPEndPoint, int>((f, a, b, c) => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Sending {c} bytes"));
			tracer.Setup(x => x.Sent(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<int>()))
				.Callback<IListenerStateData, IdentityUserToken, IPEndPoint, int>((f, a, b, c) => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Sent {c} bytes"));
			tracer.Setup(x => x.TimingOutConnection(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Timed out connection."));
			tracer.Setup(x => x.StartListen(It.IsAny<SocketListenerSettings<IListenerStateData>>()))
				.Callback<SocketListenerSettings<IListenerStateData>>(a => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Listener started: {JsonConvert.SerializeObject(a)}"));
			tracer.Setup(x => x.Dispose(It.IsAny<IListenerStateData>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Disposed."));
			tracer.Setup(x => x.QueuedTask(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Task Queued"));
			tracer.Setup(x => x.StartingTask(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<DateTime>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Task Started"));
			tracer.Setup(x => x.FailedTask(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<Exception>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Task Failed"));
			tracer.Setup(x => x.ExpiredTask(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Task Expired"));
			tracer.Setup(x => x.CompletedTask(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Task Completed"));
			tracer.Setup(x => x.Received(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
				.Callback<IListenerStateData, IdentityUserToken, IPEndPoint, byte[], int, int>((f, a, b, c, d, e) => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Data Received: {e}"));
			tracer.Setup(x => x.HandleAccept(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>()))
				.Callback<IListenerStateData, IdentityUserToken, IPEndPoint>((f, a, b) => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Connection accepted"));
			tracer.Setup(x => x.HandleBadAccept(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<SocketError>()))
				.Callback(() => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Bad SocketAccept"));
			tracer.Setup(x => x.ClosingConnection(It.IsAny<IListenerStateData>(), It.IsAny<IdentityUserToken>(), It.IsAny<IPEndPoint>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<SocketError>()))
				.Callback<IListenerStateData, IdentityUserToken, IPEndPoint, bool, bool, SocketError>((f, a, b, c, d, e) => Console.WriteLine($"S[{DateTime.Now.Ticks / ticksPerSecond}]Closing Connection: ToldToClose: {c} ClosedByClient:{d} CloseReason:{e}"));

			using (var socketListener = new SocketListener<TestingSocketStateData, IListenerStateData>(listenSettings, mockMessageHandler, tracer.Object))
			{
				socketListener.StartListen();

				var socket1 = new TcpClient();
				socket1.Connect("localhost", TestPort);

				var stream1 = socket1.GetStream();
				SendMessage(stream1,
					"GET /process.aspx?data=351777047213077|OGeNuj4NrSR00cY96P5E3Rc04h120Qd-000O001n0c1x HTTP/1.1\r\n" +
					"Host: 10.1.224.2\r\n" +
					"\r\n");
				Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Waiting for data.");
				WaitForResponse(stream1);
				Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Data received, checking that there is no data left.");
				WaitForNoResponse(stream1);
				Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]No data left.");
				Thread.Sleep(1000);
				Task.Delay(1000);
				Assert.AreEqual(1, mockMessageHandler.HandledMessages);
				stream1.Close();
				socket1.Close();

				Console.WriteLine();
				Thread.Sleep(1000);

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream, "");
						WaitForNoResponse(stream);
						Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Waiting for no response.");
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
						Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Waiting for no response.");
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
						Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Waiting for final data.");
						try
						{

							WaitForResponse(stream);
						}
						catch (Exception)
						{
							Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Final data exception thrown.");
							throw;
						}

						Console.WriteLine($"C[{DateTime.Now.Ticks / ticksPerSecond}]Final data received.");
						Assert.AreEqual(2, mockMessageHandler.HandledMessages);
					}
				}
			}
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(true, true)]
		public void TestConnectionCloseAfterProcess(bool sendDataInResponse, bool disconnectOnceDone)
		{
			var listenSettings = new SocketListenerSettings<IListenerStateData>
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

			var mockMessageHandler = new Moq.Mock<IMessageHandler<TestingSocketStateData, IListenerStateData>>();
			mockMessageHandler.Setup(x => x.IsMessageComplete(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TestingSocketStateData>())).Returns(() => true);
			mockMessageHandler.Setup(x => x.HandleMessage(It.IsAny<IPEndPoint>(), It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IListenerStateData> (), It.IsAny<TestingSocketStateData>()))
				.Returns(() => new HandleMessageResponse { DisconnectOnceDone = disconnectOnceDone, ToSend = sendDataInResponse ? Encoding.ASCII.GetBytes("hello") : null });

			using (var socketListener = new SocketListener<TestingSocketStateData, IListenerStateData>(listenSettings, mockMessageHandler.Object))
			{
				socketListener.StartListen();

				using (var socket = new TcpClient())
				{
					socket.Connect("localhost", TestPort);

					using (var stream = socket.GetStream())
					{
						SendMessage(stream, "hi");

						stream.ReadTimeout = 3000;

						var bytes = new byte[2000];
						int readLength = -1;
						bool hadException = false;
						try
						{
							readLength = stream.Read(bytes, 0, bytes.Length);
						}
						catch (IOException)
						{
							hadException = true;
						}

						bool hadException2 = false;
						try
						{
							stream.Read(bytes, 0, bytes.Length);
						}
						catch (IOException)
						{
							hadException2 = true;
						}

						if (sendDataInResponse && disconnectOnceDone)
						{
							//Get data
							Assert.AreEqual(5, readLength);
							//Get disconnected on second read.
							Assert.IsFalse(hadException2);
						}

						if (!sendDataInResponse && disconnectOnceDone)
						{
							//Get data
							Assert.AreEqual(0, readLength);
							//Get disconnected on second read.
							Assert.IsFalse(hadException2);
						}

						if (!sendDataInResponse && !disconnectOnceDone)
						{
							//Get data, but never executed due to exception
							Assert.AreEqual(-1, readLength);
							//Get disconnected and there was no data to read.
							Assert.IsTrue(hadException);
							Assert.IsTrue(hadException2);
						}

						if (sendDataInResponse && !disconnectOnceDone)
						{
							//Get data
							Assert.AreEqual(5, readLength);
							//Get disconnected on second read.
							Assert.IsFalse(hadException);
							Assert.IsTrue(hadException2);
						}
					}
				}
			}
		}
	}

	public class TestingSocketStateData : ISocketStateData
	{
		public void Reset()
		{
		}
	}

	class IPAddressConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(IPAddress));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IPAddress.Parse((string)reader.Value);
		}
	}

	class IPEndPointConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(IPEndPoint));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			IPEndPoint ep = (IPEndPoint)value;
			JObject jo = new JObject();
			jo.Add("Address", JToken.FromObject(ep.Address, serializer));
			jo.Add("Port", ep.Port);
			jo.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
			int port = (int)jo["Port"];
			return new IPEndPoint(address, port);
		}
	}
}
