namespace BaseTools.Core.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Information about relation between abstract type and it real implementation.
    /// </summary>
    internal abstract class TypeBindingBase
    {
        internal abstract object GetRealInstance();
    }
}
