using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;

namespace TestFsmActionTimeoutWithSysTimer
{
    internal class MockStateMachineDriver
    {

        #region Constructors
        public MockStateMachineDriver()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += OnTimedEvent;
            _fsmDriver = new ActionBlock<Event>(OnEventTrigger);
        }
        #endregion //Constructors

        #region Types
        #endregion //Types

        #region Fields
        private System.Timers.Timer _timer;
        private ActionBlock<Event> _fsmDriver;
        #endregion //Fields

        #region Properties
        #endregion //Properties

        #region Methods
        public void PostEvent(Event e)
        {
            _fsmDriver.Post(e);
        }

        #region Private Methods
        private void OnEventTrigger(Event e)
        { 
            _timer.Interval = e.Timeout.TotalMilliseconds < 0 ? double.MaxValue : e.Timeout.TotalMilliseconds;
            _timer.Start();

            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Processing event: {e.Name}");
            e.EventAction?.Invoke();
            _timer.Stop();
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            // Handle the timeout event here
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] The operation has timed out.");
            _fsmDriver.Post(new Event("TimeoutEvent"));
        }
        #endregion //Private Methods
        #endregion //Methods


    }
}
