using System.Collections.Generic;

namespace System.Net.Http.Contrib
{
	public static class CloneRequest
	{
		/// <summary>
		/// Copies the input request and return a new request with all of the
		/// properties that the old one had but pointing to a new URL
		/// Refs http://stackoverflow.com/q/21467018/5209435
		/// </summary>
		/// <param name="req">The original request</param>
		/// <param name="newUri">The url to go to</param>
		/// <returns></returns>
		public static HttpRequestMessage Clone(this HttpRequestMessage req, string newUri)
		{
			HttpRequestMessage clone = new HttpRequestMessage(req.Method, newUri);

			if (req.Method != HttpMethod.Get)
			{
				clone.Content = req.Content;
			}
			clone.Version = req.Version;

			foreach (KeyValuePair<string, object> prop in req.Properties)
			{
				clone.Properties.Add(prop);
			}

			foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
			{
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			return clone;
		}
	}
}
