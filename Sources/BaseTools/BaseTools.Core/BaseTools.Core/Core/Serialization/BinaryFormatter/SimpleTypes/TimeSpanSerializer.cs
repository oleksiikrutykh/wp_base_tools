namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class TimeSpanSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (TimeSpan)data;
            writer.Write(value.Ticks);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            var timeSpan = new TimeSpan(ticks);
            return timeSpan;
        }
    }
}
