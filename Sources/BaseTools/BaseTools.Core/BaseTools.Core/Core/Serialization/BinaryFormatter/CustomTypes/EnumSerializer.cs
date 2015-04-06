namespace BaseTools.Core.Serialization.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class EnumSerializer : AbstractTypeSerializer
    {
        private Type enumType;

        public EnumSerializer(Type type)
        {
            this.enumType = type;
        }

        public static AbstractTypeSerializer TryCreate(Type objectType, TypeInfo typeInfo)
        {
            AbstractTypeSerializer seriazlier = null;
            if (typeInfo.IsEnum)
            {
                seriazlier = new EnumSerializer(objectType);
            }

            return seriazlier;
        }

        public override void Serialize(System.IO.BinaryWriter writer, object data)
        {
            var longValue = (long)Convert.ChangeType(data, typeof(long));
            writer.Write(longValue);
        }

        public override object Deserialzie(System.IO.BinaryReader reader)
        {
            var longEnumValue = reader.ReadInt64();
            var result = Enum.ToObject(this.enumType, longEnumValue);
            return result;
        }
    }
}
