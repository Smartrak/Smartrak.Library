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
	}
}
