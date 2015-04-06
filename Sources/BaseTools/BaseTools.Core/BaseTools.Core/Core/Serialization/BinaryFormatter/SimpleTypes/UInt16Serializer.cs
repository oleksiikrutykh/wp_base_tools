namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class UInt16Serializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (UInt16)data;
            writer.Write(value);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }
    }
}
