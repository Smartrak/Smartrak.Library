using System.Linq;
using System.Text;
using NUnit.Framework;

namespace System.IO.Contrib.Tests
{
	[TestFixture]
	public class StreamTests
	{
		public Stream GenerateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		[Test]
		public void TestFirstPage()
		{
			var result = StreamHelpers.ReadLines(() => GenerateStreamFromString(@"aaaaa
bbbbb
ccccc"), Encoding.UTF8).ToArray();
			Assert.AreEqual(3, result.Count());
			Assert.AreEqual("aaaaa", result[0]);
			Assert.AreEqual("bbbbb", result[1]);
			Assert.AreEqual("ccccc", result[2]);
		}
	}
}
