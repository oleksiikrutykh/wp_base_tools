namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal abstract class AbstractTypeSerializer
    {
        public abstract void Serialize(BinaryWriter writer, object data);

        public abstract object Deserialzie(BinaryReader reader);
    }
}
