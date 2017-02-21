using FluentAssertions;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Contrib;

namespace System.Contrib.Tests
{
	[TestFixture]
	public class TaskExtensionsTests
	{
		[Test]
		public void LongTaskTimesOut()
		{
			var task = Task.Run(() => { Thread.Sleep(1000); return 1; });

			Assert.Throws<AggregateException>(() => task.TimeoutAfter(new System.TimeSpan(1)).Wait());
		}

		private static int _finishedTaskTimesDoesntExecuteAgainCounter = 0;
		[Test]
		public void FinishedTaskTimesDoesntExecuteAgain()
		{
			var task = Task.Run(() => { _finishedTaskTimesDoesntExecuteAgainCounter++; return 1; });

			Thread.Sleep(10);
			task.TimeoutAfter(new System.TimeSpan(1)).Wait();

			Assert.AreEqual(1, _finishedTaskTimesDoesntExecuteAgainCounter);
		}

		[Test]
		public void TaskCompletesWithoutReachingTimeout()
		{

			var task = Task.Run(() => { Thread.Sleep(50); return 1; });
			var sw = Stopwatch.StartNew();
			task.TimeoutAfter(new System.TimeSpan(0, 0, 0, 0, 10000)).Wait();
			sw.ElapsedMilliseconds.Should().BeLessThan(100);
		}
	}
}
