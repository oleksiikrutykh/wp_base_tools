namespace BaseTools.UI.Common
{
    using BaseTools.Core.Threading;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class PerformanceDispatcher
    {
        private static PerformanceDispatcher instance = new PerformanceDispatcher();

        private readonly object syncRoot = new object();

        private TaskCompletionSource<bool> heavyTaskDelay;

        private List<Releaser> currentPerfromanceOperations = new List<Releaser>();

        private List<Releaser> heavyOperations = new List<Releaser>();

        private static readonly Task completedTask = Task.FromResult<bool>(false);
       

        public static PerformanceDispatcher Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<Releaser> ExecuteHeavyUIOperation()
        {
            Task waiter = completedTask;
            if (this.heavyTaskDelay != null)
            {
                waiter = this.heavyTaskDelay.Task;
            }

            await waiter;
            var releaser = new Releaser();
            return releaser;
        }

        public Releaser ExecutePerformanceOperation()
        {
            var releaser = new Releaser();
            lock (syncRoot)
            {
                releaser.Disposed += (s, e) =>
                {
                    lock (syncRoot)
                    {
                        this.currentPerfromanceOperations.Remove(releaser);
                        if (this.currentPerfromanceOperations.Count == 0)
                        {
                            this.heavyTaskDelay.SetResult(true);
                            this.heavyTaskDelay = null;
                        }
                    }
                };

                if (this.heavyTaskDelay == null)
                {
                    this.heavyTaskDelay = new TaskCompletionSource<bool>();
                }

                this.currentPerfromanceOperations.Add(releaser);
            }

            return releaser;
        }
    }
}
