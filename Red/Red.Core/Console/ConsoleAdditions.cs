using System;
using System.IO;

namespace Red.Core.Console
{
    public class ConsoleHelper
    {
        public static string CaptureConsoleOutput(Action action)
        {
            var storedOut = System.Console.Out;
            using (var sw = new StringWriter())
            {
                try
                {
                    System.Console.SetOut(sw);
                    action();
                    return sw.ToString();
                }
                finally
                {
                    System.Console.SetOut(storedOut);
                }
            }
        }

    }
}
