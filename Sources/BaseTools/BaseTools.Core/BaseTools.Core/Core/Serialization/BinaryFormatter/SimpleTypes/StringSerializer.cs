namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class StringSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            string value = (string)data;
            var hasValue = value != null;
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write(value);
            }
        }

        public override object Deserialzie(BinaryReader reader)
        {
            string value = null;
            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                value = reader.ReadString();
            }

            return value;
        }
    }

}
