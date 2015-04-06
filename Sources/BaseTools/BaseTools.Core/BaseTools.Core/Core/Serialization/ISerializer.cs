namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Allows to store and restore data from stream. 
    /// </summary>
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T storedObj);

        T Deserialize<T>(Stream stream);
    }
}
