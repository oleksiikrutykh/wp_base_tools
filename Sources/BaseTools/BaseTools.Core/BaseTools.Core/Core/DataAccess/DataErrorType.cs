namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the category of error that can be occured during request
    /// </summary>
    public enum DataErrorType
    {
        Unknown = 0,

        Network = 1,

        NoInternetConnection = 2,

        Parsing = 3,

        Custom = 4,

        InvalidRequest = 5,

        NoContent = 6,

        InvalidUI = 7,

        UnautorizedAccess = 8,

        Cancelled = 9,
    }
}
