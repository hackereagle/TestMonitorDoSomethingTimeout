using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMonitorDoSomethingTimeout.TargetModules
{
    public class ExecutionProxy
    {
		#region Constructors
		#endregion Constructors

		#region Types
		#endregion Types

		#region Fields
		#endregion Fields

		#region Properties
		#endregion Properties

		#region Methods
		// Refer to https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout
		public static async Task Execute(Func<Task> method, TimeSpan timeout, string label = "")
		{
			using var cts = new CancellationTokenSource();
			Task timeoutMonitor = Task.Delay(timeout, cts.Token);
			//Task timeoutMonitor = Task.Delay(timeout);
			Task task = method();
			if (await Task.WhenAny(task, timeoutMonitor) == task)
			{
				cts.Cancel();
				await task;
			}
			else
			{ 
				throw new TimeoutException($"{label} timeout!");
			}
		}

		public static async Task Execute(Action action, TimeSpan timeout, string label = "")
		{ 
			using var ctsForTimeoutMonitorTask = new CancellationTokenSource();
			Task timeoutMonitor = Task.Delay(timeout, ctsForTimeoutMonitorTask.Token);
			//Task timeoutMonitor = Task.Delay(timeout);
			using var ctsForAction = new CancellationTokenSource();
			Task task = Task.Run(action, ctsForAction.Token);
			if (await Task.WhenAny(task, timeoutMonitor) == task)
			{
				ctsForTimeoutMonitorTask.Cancel();
				await task;
			}
			else
			{ 
				ctsForAction.Cancel();
				throw new TimeoutException($"{label} timeout!");
			}
		}
		#endregion Methods

    }
}
