namespace BaseTools.Core.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Allows to perform multiple binding operations.
    /// </summary>
    public abstract class FactoryInitializer
    {
        public abstract void SetupBindnings(Factory initializedFactory);
    }
}
