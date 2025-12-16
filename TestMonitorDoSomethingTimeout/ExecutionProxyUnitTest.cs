using System.Diagnostics;
using TestMonitorDoSomethingTimeout.TargetModules;

namespace TestMonitorDoSomethingTimeout
{
    public class ExecutionProxyUnitTest
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
		[SetUp]
        public void Setup()
        {
        }

        #region Test Methods
        [Test]
        public void TestDoSomething_Timeout()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(1);

            // Act and Assert
            Assert.ThrowsAsync<TimeoutException>(
                async () => await ExecutionProxy.Execute(DoSomethingAsync, timeout));
        }

        [Test]
        public void TestDoSomethingWithoutTimeout_GetException()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(60);

            // Act and Assert
            Assert.ThrowsAsync<Exception>(
                async () => await ExecutionProxy.Execute(DoSomethingAsyncWithException, timeout));
        }

        [Test]
        public void TestRunMethodWith_Timeout()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(1);

            // Act and Assert
            Assert.ThrowsAsync<TimeoutException>(
                async () => await ExecutionProxy.Execute(DoSomething, timeout));
        }

        [Test]
        public void TestRunMethodWithoutTimeout_GetException()
        {
            // Arrange
            var timeout = TimeSpan.FromSeconds(60);

            // Act and Assert
            Assert.ThrowsAsync<Exception>(
                async () => await ExecutionProxy.Execute(DoSomethingWithException, timeout));
        }
        #endregion Test Methods

        #region Private Methods
        private async Task DoSomethingAsync()
        { 
            Console.WriteLine("DoSomethingAsync: Start");
            await Task.Delay(2000);
            Console.WriteLine("DoSomethingAsync: End");
        }

        private async Task DoSomethingAsyncWithException()
        {
            Console.WriteLine("DoSomethingAsyncWithException: Start");
            await Task.Delay(1000);
            throw new Exception("DoSomethingAsyncWithException: Exception");
        }

        private void DoSomething()
        {
            Console.WriteLine("DoSomething: Start");
            Thread.Sleep(2000);
            Console.WriteLine("DoSomething: End");
        }

        private void DoSomethingWithException()
        {
            Console.WriteLine("DoSomethingWithException: Start");
            Thread.Sleep(1000);
            throw new Exception("DoSomethingWithException: Exception");
        }
        #endregion Private Methods
        #endregion Methods

    }
}