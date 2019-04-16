namespace PerformantSocketServer
{
	using System;
	
	public class IdentityUserToken
	{
		internal IdentityUserToken()
		{
			Id = Guid.NewGuid();
			LastTalked = ServerDiagnostics.Instance.Now;
		}

		public Guid Id { get; private set; }
		public long LastTalked { get; internal set; }
	}
}
