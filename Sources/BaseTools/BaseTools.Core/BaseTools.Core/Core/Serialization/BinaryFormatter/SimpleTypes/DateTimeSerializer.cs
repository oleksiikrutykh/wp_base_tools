namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class DateTimeSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (DateTime)data;
            writer.Write(value.Ticks);
            writer.Write((byte)value.Kind);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            var kind = (DateTimeKind)reader.ReadByte();
            var dateTime = new DateTime(ticks, kind);
            return dateTime;
        }
    }
}
