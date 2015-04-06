namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class CharSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (char)data;
            writer.Write(value);
        }

        public override object Deserialzie(BinaryReader reader)
        {
            return reader.ReadChar();
        }
    }
}
