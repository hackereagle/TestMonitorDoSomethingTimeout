using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFsmActionTimeoutWithSysTimer
{
    class Event
    {
        public Event(string name, Action? eventAction = null, TimeSpan? timeout = null)
        {
            Name = name;
            EventAction = eventAction;
            if (timeout == null)
                Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            else
                Timeout = timeout.Value;
        }

        public string Name { get; private set; }
        public Action? EventAction { get; private set;}
        public TimeSpan Timeout { get; private set; }
    }
}
