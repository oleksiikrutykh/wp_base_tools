namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Perform data validation check.
    /// </summary>
    public static class Guard
    {
        public static void CheckIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
