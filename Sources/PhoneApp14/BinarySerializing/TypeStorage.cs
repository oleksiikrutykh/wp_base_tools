namespace BinarySerializing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using BinarySerializing.SimpleTypes;
    using BinarySerializing.CustomTypes;

    internal static class TypeStorage
    {
        private static Dictionary<Type, AbstractTypeSerializer> allKnownTypes = new Dictionary<Type, AbstractTypeSerializer>();

        private static List<Func<Type, TypeInfo, AbstractTypeSerializer>> serializerGenerators = new List<Func<Type, TypeInfo, AbstractTypeSerializer>>();

        //private static Dictionary<Type, bool> hasDataContractAttribute = new Dictionary<Type, bool>();

        static TypeStorage()
        {
            allKnownTypes[typeof(bool)] = new BooleanSerializer();
            allKnownTypes[typeof(byte)] = new ByteSerializer();
            allKnownTypes[typeof(char)] = new CharSerializer();
            allKnownTypes[typeof(Decimal)] = new DecimalSerializer();
            allKnownTypes[typeof(double)] = new DoubleSerializer();
            allKnownTypes[typeof(short)] = new Int16Serializer();
            allKnownTypes[typeof(int)] = new Int32Serializer();
            allKnownTypes[typeof(long)] = new Int64Serializer();
            allKnownTypes[typeof(SByte)] = new SByteSerializer();
            allKnownTypes[typeof(Single)] = new SingleSerializer();
            allKnownTypes[typeof(string)] = new StringSerializer();
            allKnownTypes[typeof(UInt16)] = new UInt16Serializer();
            allKnownTypes[typeof(UInt32)] = new UInt32Serializer();
            allKnownTypes[typeof(UInt64)] = new UInt64Serializer();
            allKnownTypes[typeof(DateTime)] = new DateTimeSerializer();
            allKnownTypes[typeof(TimeSpan)] = new TimeSpanSerializer();
            allKnownTypes[typeof(Uri)] = new UriSerializer();

            serializerGenerators.Add(ArrayTypeSerializer.TryCreate);
            serializerGenerators.Add(DictionarySerializer.TryCreate);
            serializerGenerators.Add(CollectionTypeSerializer.TryCreate);
            serializerGenerators.Add(NullableSerializer.TryCreate);
            serializerGenerators.Add(EnumSerializer.TryCreate);
            serializerGenerators.Add(CustomTypeSerializer.TryCreate);
        }

        internal static AbstractTypeSerializer GetForType(Type type)
        {
            AbstractTypeSerializer serializingType = null;
            var isExist = allKnownTypes.TryGetValue(type, out serializingType);
            if (!isExist)
            {
                serializingType = Attach(type);
            }

            return serializingType;
        }

        private static AbstractTypeSerializer Attach(Type type)
        {
            AbstractTypeSerializer serializer = null;

            var typeInfo = type.GetTypeInfo();
            foreach (var generator in serializerGenerators)
            {
                serializer = generator.Invoke(type, typeInfo);
                if (serializer != null)
                {
                    break;
                }
            }

            if (serializer == null)
            {
                throw new InvalidOperationException("Can't serialize type " + type.FullName);
            }

            allKnownTypes[type] = serializer;
            return serializer;
        }

    }

}
