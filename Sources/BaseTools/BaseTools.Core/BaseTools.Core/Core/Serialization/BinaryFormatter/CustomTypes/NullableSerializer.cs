namespace BaseTools.Core.Serialization.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class NullableSerializer : AbstractTypeSerializer
    {
        private LazyLoadSerializer valueSerializer;

        public NullableSerializer(Type nullableValueType)
        {
            this.valueSerializer = new LazyLoadSerializer(nullableValueType);
        }

        public static AbstractTypeSerializer TryCreate(Type type, TypeInfo typeInfo)
        {
            AbstractTypeSerializer serializer = null;
            if (typeInfo.IsGenericType)
            {
                var isNullable = typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (isNullable)
                {
                    var valueType = typeInfo.GenericTypeArguments[0];
                    serializer = new NullableSerializer(valueType);
                }
            }

            return serializer;
        }

        public override object Deserialzie(System.IO.BinaryReader reader)
        {
            object result = null;
            var hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                result = this.valueSerializer.Serializer.Deserialzie(reader);
            }

            return result;
        }

        public override void Serialize(System.IO.BinaryWriter writer, object data)
        {
            var hasValue = data != null;
            writer.Write(hasValue);
            if (hasValue)
            {
                this.valueSerializer.Serializer.Serialize(writer, data);
            }
        }
    }
}
