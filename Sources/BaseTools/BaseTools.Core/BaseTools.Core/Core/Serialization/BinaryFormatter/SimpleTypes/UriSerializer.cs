namespace BaseTools.Core.Serialization.SimpleTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class UriSerializer : AbstractTypeSerializer
    {
        public override void Serialize(BinaryWriter writer, object data)
        {
            var value = (Uri)data;
            bool hasValue = value != null;
            writer.Write(hasValue);
            if (value != null)
            {
                writer.Write(value.OriginalString);
                writer.Write(value.IsAbsoluteUri);
            }
        }

        public override object Deserialzie(BinaryReader reader)
        {
            Uri uri = null;
            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                var originalString = reader.ReadString();
                var kind = UriKind.Relative;
                var isAbsoluteUri = reader.ReadBoolean();
                if (isAbsoluteUri)
                {
                    kind = UriKind.Absolute;
                }

                uri = new Uri(originalString, kind);
            }

            return uri;
        }
    }
}
