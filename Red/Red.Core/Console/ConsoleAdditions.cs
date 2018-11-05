using System;
using System.IO;

namespace Red.Core
{
    public class ConsoleHelper
    {
        public static string CaptureConsoleOutput(Action action)
        {
            var storedOut = Console.Out;
            using (var sw = new StringWriter())
            {
                try
                {
                    Console.SetOut(sw);
                    action();
                    return sw.ToString();
                }
                finally
                {
                    Console.SetOut(storedOut);
                }
            }
        }

    }
}
