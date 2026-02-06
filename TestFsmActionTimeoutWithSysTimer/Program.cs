using System.Threading.Tasks.Dataflow;
using TestFsmActionTimeoutWithSysTimer;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("===== Simulate =====");
        MockStateMachineDriver driver = new MockStateMachineDriver();

        Event begin = new Event( "Begin", () => { Thread.Sleep(3000); }, TimeSpan.FromSeconds(1));
        driver.PostEvent(begin);

        Thread.Sleep(5000);
        Event evnt = new Event( "Event", () => { Console.WriteLine("Do something"); }, TimeSpan.FromSeconds(3));
        driver.PostEvent(evnt);

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }
}