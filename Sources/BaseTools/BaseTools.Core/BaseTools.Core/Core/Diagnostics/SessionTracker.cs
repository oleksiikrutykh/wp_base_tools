namespace BaseTools.Core.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SessionLogger
    {
        public static readonly SessionLogger Instance = new SessionLogger();

        private Stopwatch sw;

        bool canWrite = false;

        public void Start(string text)
        {
            if (canWrite)
            {
                Debug.WriteLine("SessionStarted: " + text);
            }

            sw = Stopwatch.StartNew();
        }

        public void Log(string text)
        {
            sw.Stop();
            if (canWrite)
            {
                Debug.WriteLine(text + ": " + sw.ElapsedMilliseconds + " ms (" + sw.ElapsedTicks + ")");
            }

            sw.Start();
        }

        public void Stop()
        {
            if (canWrite)
            {
                Debug.WriteLine("Stopped");
            }

            sw.Stop();
        }
    }
}
