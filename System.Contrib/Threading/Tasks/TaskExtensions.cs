namespace System.Threading.Tasks.Contrib
{
	/// <summary>
	/// Extensions on top of System.Threading.Tasks.Task
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Waits for a task for a maximum time and then throws an exception. Useful if you don't want to wait forever for a hanging task.
		/// NOTE: this does not cancel the inner task if it doesn't complete on time but it does return control.
		/// From: http://stackoverflow.com/a/22078975/1070291
		/// </summary>
		/// <typeparam name="TResult">The result type of the subject task</typeparam>
		/// <param name="task">the subject task</param>
		/// <param name="timeout">The maximum time you want to wait before giving up on the task</param>
		/// <returns>The result of the subject task</returns>
		public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
		{
			var timeoutCancellationTokenSource = new CancellationTokenSource();

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
			if (completedTask == task)
			{
				timeoutCancellationTokenSource.Cancel();
				return await task;
			}
			else
			{
				throw new TimeoutException("The operation has timed out.");
			}
		}
	}
}
