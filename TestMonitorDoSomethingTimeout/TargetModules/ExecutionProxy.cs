using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace TestMonitorDoSomethingTimeout.TargetModules
{

    public enum CAACK
    {
        //CAACK 51 Carrier Action Acknowledge Code, 1 byte.

        CommandPerformed, //0 = Acknowledge, command has been performed.
        InvalidCommand, //1 = Invalid command.
        CanNotPerformNow, //2 = Can not perform now.
        InvalidDataOrArgument, //3 = Invalid data or argument.
        willBePerformedByAnEvent, //4 = Acknowledge, request will be performed with completion signaled later by an event.
        Rejected, //5 = Rejected.Invalid state.
        CommandPerformedWithErrors, //6 = Command performed with errors.
                                    //7-63 = Reserved.
    }

    public class ExecutionProxy
    {

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
				throw new TimeoutException($"{label} timeout! Over {timeout}");
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
				throw new TimeoutException($"{label} timeout! Over {timeout}");
			}
		}

		public static async Task Execute(IEnumerable<Task> tasks, TimeSpan timeout, string label = "")
		{ 
			using var ctsForTimeoutMonitorTask = new CancellationTokenSource();
			using var ctsForAction = new CancellationTokenSource();

			var timeoutMonitor = Task.Delay(timeout, ctsForTimeoutMonitorTask.Token);
			var allTasksWaiter = Task.WhenAll(tasks);

			var completedTask = await Task.WhenAny(allTasksWaiter, timeoutMonitor);
			if (completedTask == timeoutMonitor)
			{
				throw new TimeoutException($"{label} timeout! Over {timeout}!");
			}
			else
			{ 
				ctsForAction.Cancel();
				await allTasksWaiter;
			}
		}

		public static async Task<CAACK> ExecuteTransaction(Func<CAACK> requestion, IObservable<int> successCallback, IObservable<int>? failCallback, TimeSpan timeout, string label = "")
		{ 
			using var ctsForTimeoutMonitorTask = new CancellationTokenSource();
			using var ctsForAction = new CancellationTokenSource();

			var successCallbackWaiter = successCallback.ObserveOn(Scheduler.Default).Take(1).ToTask(ctsForAction.Token);
			Task? failCallbackWaiter = null;
			if (failCallback != null)
				failCallbackWaiter = failCallback.ObserveOn(Scheduler.Default).Take(1).ToTask(ctsForAction.Token);
			var timeoutMonitor = Task.Delay(timeout, ctsForTimeoutMonitorTask.Token);

			var ack = requestion();
			//logger?.Debug($"{label} request result = {ack}");
			if (CAACK.willBePerformedByAnEvent != ack)
			{
				ctsForAction.Cancel();
				ctsForTimeoutMonitorTask.Cancel();
				return ack;
			}

			Task completedTask;
			if (failCallbackWaiter != null)
				completedTask = await Task.WhenAny(successCallbackWaiter, failCallbackWaiter, timeoutMonitor);
			else
				completedTask = await Task.WhenAny(successCallbackWaiter, timeoutMonitor);

			if (timeoutMonitor == completedTask)
			{
				ctsForAction.Cancel();
				throw new TimeoutException($"{label} timeout! Over {timeout}!");
			}
			else
			{
				ctsForTimeoutMonitorTask.Cancel();
				if (successCallbackWaiter == completedTask)
				{
					return CAACK.CommandPerformed;
				}
				else
				{
					return CAACK.CommandPerformedWithErrors;
				}
			}
		}
		#endregion Methods

    }
}
