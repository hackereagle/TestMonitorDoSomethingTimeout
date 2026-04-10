using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMonitorDoSomethingTimeout.TargetModules;
using TestMonitorDoSomethingTimeout.TestModules;

namespace TestMonitorDoSomethingTimeout
{
    internal class ExecuteTrasactionUnitTest
    {
        [Test]
        public async Task TestTransaction_ThereAreBothSuccessAndFailCallback_Success()
        {
            // Arrange
            Service service = new Service();

            // Act
            var res = await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething(needReject: false, needFail: false, simulationWorkTime: TimeSpan.FromSeconds(1)),
                service.DoSomethingSuccessObservable,
                service.DoSomethingFailObservable,
                timeout: TimeSpan.FromSeconds(500),
                $"{nameof(TestTransaction_ThereAreBothSuccessAndFailCallback_Success)}");

            // Assert
            Assert.IsTrue(res == CAACK.CommandPerformed);
        }

        [Test]
        public async Task TestTransaction_ThereAreBothSuccessAndFailCallback_Fail()
        {
            // Arrange
            Service service = new Service();

            // Act
            var res = await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething(needReject: false, needFail: true, simulationWorkTime: TimeSpan.FromSeconds(1)),
                service.DoSomethingSuccessObservable,
                service.DoSomethingFailObservable,
                timeout: TimeSpan.FromSeconds(500),
                $"{nameof(TestTransaction_ThereAreBothSuccessAndFailCallback_Fail)}");

            // Assert
            Assert.IsTrue(res == CAACK.CommandPerformedWithErrors);
        }

        [Test]
        public void TestTransaction_ThereAreBothSuccessAndFailCallback_Timeout()
        {
            // Arrange
            Service service = new Service();

            // Act And Assert
            Assert.ThrowsAsync<TimeoutException>(async () =>
             await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething(needReject: false, needFail: false, simulationWorkTime: TimeSpan.FromSeconds(10)),
                service.DoSomethingSuccessObservable,
                service.DoSomethingFailObservable,
                timeout: TimeSpan.FromSeconds(5),
                $"{nameof(TestTransaction_ThereAreBothSuccessAndFailCallback_Timeout)}"));

        }

        [Test]
        public async Task TestTransaction_ThereAreBothSuccessAndFailCallback_Reject()
        {
            // Arrange
            Service service = new Service();

            // Act
             var ack = await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething(needReject: true, needFail: false, simulationWorkTime: TimeSpan.FromSeconds(1)),
                service.DoSomethingSuccessObservable,
                service.DoSomethingFailObservable,
                timeout: TimeSpan.FromSeconds(5),
                $"{nameof(TestTransaction_ThereAreBothSuccessAndFailCallback_Reject)}");

            // Assert
            Assert.IsTrue(ack == CAACK.Rejected);
        }

        [Test]
        public async Task TestTransaction_ThereIsOnlySuccessCallback_Success()
        {
            // Arrange
            Service service = new Service();

            // Act And Assert
            var res = await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething2(needReject: false, needFail: false, simulationWorkTime: TimeSpan.FromSeconds(1)),
                service.DoSomething2SuccessObservable,
                null,
                timeout: TimeSpan.FromSeconds(5),
                $"{nameof(TestTransaction_ThereIsOnlySuccessCallback_Success)}");

            // Assert
            Assert.IsTrue(res == CAACK.CommandPerformed);
        }

        [Test]
        public void TestTransaction_ThereIsOnlySuccessCallback_Fail()
        {
            // Arrange
            Service service = new Service();

            // Act And Assert
            Assert.ThrowsAsync<TimeoutException>(async () =>
             await ExecutionProxy.ExecuteTransaction(
                () => service.DoSomething2(needReject: false, needFail: true, simulationWorkTime: TimeSpan.FromSeconds(1)),
                service.DoSomething2SuccessObservable,
                null,
                timeout: TimeSpan.FromSeconds(5),
                $"{nameof(TestTransaction_ThereIsOnlySuccessCallback_Fail)}"));
        }
    }
}
