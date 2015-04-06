namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Perform convertions between toolkit <see cref="FileOpeningMode"/>  and system <see cref="System.IO.FileMode"/>
    /// </summary>
    internal static class FileModeConverter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Can be used in future.")]
        internal static FileOpeningMode ConvertFromSystem(System.IO.FileMode mode)
        {
            var integerValue = (int)mode;
            var result = (FileOpeningMode)integerValue;
            return result;
        }

        internal static System.IO.FileMode ConvertToSystem(FileOpeningMode mode)
        {
            var integerValue = (int)mode;
            var result = (System.IO.FileMode)integerValue;
            return result;
        }
    }
}
