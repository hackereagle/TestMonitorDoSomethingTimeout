using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TestMonitorDoSomethingTimeout.TargetModules;

namespace TestMonitorDoSomethingTimeout.TestModules
{
    internal class Service
    {
        public Service()
        { 
        }

        private Subject<int> _doSomethingSuccessSubject = new Subject<int>();
        public IObservable<int> DoSomethingSuccessObservable => _doSomethingSuccessSubject.AsObservable();
        private Subject<int> _doSomethingFailSubject = new Subject<int>();
        public IObservable<int> DoSomethingFailObservable => _doSomethingFailSubject.AsObservable();

        public CAACK DoSomething(bool needReject, bool needFail, TimeSpan simulationWorkTime)
        { 
            if (needReject)
                return CAACK.Rejected;

            Task.Run(async () =>
            { 
                await Task.Delay(simulationWorkTime);
                if (needFail)
                    _doSomethingFailSubject.OnNext(1);
                else
                    _doSomethingSuccessSubject.OnNext(1);
            });
            return CAACK.willBePerformedByAnEvent;
        }

        private Subject<int> _doSomething2SuccessSubject = new Subject<int>();
        public IObservable<int> DoSomething2SuccessObservable => _doSomething2SuccessSubject.AsObservable();
        public CAACK DoSomething2(bool needReject, bool needFail, TimeSpan simulationWorkTime)
        {
            if (needReject)
                return CAACK.Rejected;

            Task.Run(async () =>
            {
                await Task.Delay(simulationWorkTime);
                if (!needFail)
                    _doSomething2SuccessSubject.OnNext(1);
            });
            return CAACK.willBePerformedByAnEvent;
        }
    }
}
