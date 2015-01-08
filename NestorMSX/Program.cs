using System;
using System.Windows.Forms;
using Konamiman.NestorMSX.Host;

namespace NestorMSX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "keytest")
            {
                Console.WriteLine("*** Key test running. Press keys to see their name. Press CTRL-C to quit.");
                var keyEventsource = new EmulatorHostForm();// KeyEventSource();
                keyEventsource.KeyPressed += (sender, keyArgs) => Console.WriteLine(keyArgs.Value.ToString());
                Console.CancelKeyPress += (sender, eventArgs) => { while (Console.KeyAvailable) Console.ReadKey(true); };
                Application.Run(keyEventsource);
                return;
            }

            (new MsxEmulator()).Run();
        }
    }
}
