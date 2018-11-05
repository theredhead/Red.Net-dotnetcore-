using System;
using System.Diagnostics;

namespace Red.Data.Tests
{
    public class BaseTestClass
    {
        protected long Time(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();

            action();

            var elapsed = sw.ElapsedMilliseconds;
            sw.Stop();
            return elapsed;
        }
    }
}
