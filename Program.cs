using System;

namespace SpeedTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Fehler: URL fehlt!");
                Console.WriteLine("Nutzung:");
                Console.WriteLine($"{nameof(SpeedTest)}.exe [URL]");
                return;
            }

            var test = new SpeedTest(args[0]);

            test.Run();
        }
    }
}
