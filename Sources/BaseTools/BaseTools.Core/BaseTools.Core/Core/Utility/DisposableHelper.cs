namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Allows to perform dispose.
    /// </summary>
    public static class DisposableHelper
    {
        public static void DisposeSafe(IDisposable instance)
        {
            if (instance != null)
            {
                instance.Dispose();
            }
        }
    }
}
