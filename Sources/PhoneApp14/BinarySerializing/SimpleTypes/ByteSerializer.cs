namespace BinarySerializing.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class ByteSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (byte)data;
            writer.Write(value);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            return reader.ReadByte();
        }
    }
}
