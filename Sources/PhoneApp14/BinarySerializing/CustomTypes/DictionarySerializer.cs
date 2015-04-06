namespace BinarySerializing.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Collections;

    internal class DictionarySerializer : TypeCheckerSerializer
    {
        private LazyLoadSerializer keySerializer;

        private LazyLoadSerializer valueSeralizer;

        public DictionarySerializer(Type objectType, TypeInfo typeInfo)
            : base(objectType)
        {
            var keyType = typeof(object);
            var valueType = typeof(object);
            foreach (var inter in typeInfo.ImplementedInterfaces)
            {
                var interTypeInfo = inter.GetTypeInfo();
                if (interTypeInfo.IsGenericType &&
                    interTypeInfo.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    var genericArguments = interTypeInfo.GenericTypeArguments;
                    keyType = genericArguments[0];
                    valueType = genericArguments[1];
                    break;
                }
            }

            this.keySerializer = new LazyLoadSerializer(keyType);
            this.valueSeralizer = new LazyLoadSerializer(valueType);
        }

        public static AbstractTypeSerializer TryCreate(Type objectType, TypeInfo typeInfo)
        {
            AbstractTypeSerializer serialzier = null;
            var implementedInterfaces = objectType.GetTypeInfo().ImplementedInterfaces;
            foreach (var inter in implementedInterfaces)
            {
                if (inter == typeof(IDictionary))
                {
                    serialzier = new DictionarySerializer(objectType, typeInfo);
                    break;
                }
            }

            return serialzier;
        }
         
        protected override object DeseralizeSafe(System.IO.BinaryReader reader)
        {
            var result = (IDictionary)Activator.CreateInstance(this.ObjectType);
            var itemsCount = reader.ReadInt32();
            for (int i = 0; i < itemsCount; i++)
            {
                var key = this.keySerializer.Serializer.Deserialzie(reader);
                var value = this.valueSeralizer.Serializer.Deserialzie(reader);
                result.Add(key, value);
            }

            return result;
        }

        protected override void SerializeSafe(System.IO.BinaryWriter writer, object data)
        {
            var dictionary = data as IDictionary;
            if (dictionary != null)
            {
                writer.Write(dictionary.Count);
                foreach (DictionaryEntry item in dictionary)
                {
                    this.keySerializer.Serializer.Serialize(writer, item.Key);
                    this.valueSeralizer.Serializer.Serialize(writer, item.Value);
                }
            }
            else
            {
                throw new InvalidOperationException("Can't serialize dictionary");
            }
        }
    }
}
