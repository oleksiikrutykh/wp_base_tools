namespace BinarySerializing.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class Int16Serializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var intValue = (short)data;
            writer.Write(intValue);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            return reader.ReadInt16();
        }
    }
}
