namespace BaseTools.Core.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Information about relation between abstract type and it real implementation.
    /// </summary>
    /// <typeparam name="TSource">The base type.</typeparam>
    /// <typeparam name="TReal">The real implementation type.</typeparam>
    internal class TypeBinding<TSource, TReal> : TypeBindingBase
        where TReal : TSource, new()
    {
        private TReal singletonInstance;

        private object singletonSyncObject = new object();

        internal override object GetRealInstance()
        {
            object result = null;
            if (this.singletonInstance == null)
            {
                lock (this.singletonSyncObject)
                {
                    if (this.singletonInstance == null)
                    {
                        this.singletonInstance = new TReal();
                    }
                }
            }

            result = this.singletonInstance;
            return result;
        }
    }
}
