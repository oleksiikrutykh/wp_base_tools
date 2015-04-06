namespace BaseTools.Core.Threading
{
    using System;
    using System.Threading.Tasks;

    public class RepeatedOperation
    {
        private bool isExecutedNow;

        private bool needRepeat;

        private readonly object locker = new object();

        public Func<Task> Operation { get; set; }

        public void Perform()
        {
            lock (this.locker)
            {
                if (!this.isExecutedNow)
                {
                    this.isExecutedNow = true;
                    PerformOperation();
                }
                else
                {
                    this.needRepeat = true;
                }
            }
        }

        private async void PerformOperation()
        {
            bool operationNotCompleted = false;
            do
            {
                operationNotCompleted = false;
                await Task.Run(async () =>
                {
                    Task currentOperation = this.Operation.Invoke();
                    await currentOperation;
                });

                lock (this.locker)
                {
                    if (this.needRepeat)
                    {
                        this.needRepeat = false;
                        operationNotCompleted = true;
                    }
                    else
                    {
                        this.isExecutedNow = false;
                    }
                }
            }
            while (operationNotCompleted);
        }
    }
}
