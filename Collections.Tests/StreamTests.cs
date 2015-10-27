using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smartrak.Collections.Streams;
using NUnit.Framework;
using System.Text;

namespace Smartrak.Collections.Tests
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
ccccc"),Encoding.UTF8).ToArray();
			Assert.AreEqual(3, result.Count());
			Assert.AreEqual("aaaaa", result[0]);
			Assert.AreEqual("bbbbb", result[1]);
			Assert.AreEqual("ccccc", result[2]);
		}
	}
}
