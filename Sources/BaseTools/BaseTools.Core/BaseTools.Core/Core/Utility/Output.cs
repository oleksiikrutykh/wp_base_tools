namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public class Output
    {
        private Stopwatch sw;

        public static readonly Output Instance = new Output();


        public void Run()
        {
            sw = Stopwatch.StartNew();
            Debug.WriteLine("Strated");
        }

        public void Write(string content)
        {
            Debug.WriteLine(content + " - " + sw.ElapsedMilliseconds);
        }
    }
}
