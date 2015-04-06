namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public static class TaskExtesions
    {
        public static async Task<bool> WaitForPeriod(this Task task, int period)
        {
            var delayTask = Task.Delay(period);
            var endedTask = await Task.WhenAny(task, delayTask);
            return endedTask == task;
        }
    }
}
