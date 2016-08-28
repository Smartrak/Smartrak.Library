using System.Collections.Generic;
using System.Text;

namespace System.IO.Contrib
{
	/// <summary>
	/// Reads all lines from a stream as an enumerable.
	/// Adapted from @jonskeet's answer here: http://stackoverflow.com/a/13312954/1070291
	/// </summary>
	public static class StreamHelpers
	{
		public static IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
		{
			using (var stream = streamProvider())
			using (var reader = new StreamReader(stream, encoding))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}

		/// <summary>
		/// Converts a stream to an array of bytes. Based on: http://stackoverflow.com/a/221941/1070291
		/// </summary>
		/// <param name="input">A stream</param>
		/// <returns>An array of bytes contained in the stream</returns>
		public static byte[] ToBytes(this Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
	}
}
