using System;

namespace MultiAsyncThreading
{
	public interface IAsyncCallbackRegister
	{
		void Add(IAsyncResult asyncResult, Action<IAsyncResult> callback);
	}
}