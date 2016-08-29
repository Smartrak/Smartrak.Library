using System.Web;

namespace System.Net.Http.Contrib
{
	public static class ClientIp
	{
		/// <summary>
		/// Gets the IP address of the client in webapi, based on http://www.herlitz.nu/2013/06/27/getting-the-client-ip-via-asp-net-web-api/
		/// </summary>
		/// <param name="request">The request to examine</param>
		/// <returns>A string version of the IP of the client or null if not found.</returns>
		public static string GetClientIpAddress(this HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
			}
			return null;
		}
	}
}
