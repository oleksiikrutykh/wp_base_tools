namespace BinarySerializing.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Collections;
    using System.IO;

    internal class CollectionTypeSerializer : TypeCheckerSerializer
    {
        private Type itemType = null;

        private AbstractTypeSerializer itemSerialzier;

        public AbstractTypeSerializer ItemSerializer
        {
            get
            {
                if (this.itemSerialzier == null)
                {
                    this.itemSerialzier = TypeStorage.GetForType(this.itemType);
                }


                return itemSerialzier;
            }
        }

        internal CollectionTypeSerializer(Type objectType, Type itemType)
            : base(objectType)
        {
            this.itemType = itemType;
        }

        public static AbstractTypeSerializer TryCreate(Type objectType, TypeInfo typeInfo)
        {
            CollectionTypeSerializer serialzier = null;
            var implementedInterfaces = objectType.GetTypeInfo().ImplementedInterfaces;
            foreach (var inter in implementedInterfaces)
            {
                if (inter == typeof(IEnumerable))
                {
                    var itemType = TryDetermineItemType(typeInfo);
                    serialzier = new CollectionTypeSerializer(objectType, itemType);
                    break;
                }
            }

            return serialzier;
        }

        private static Type TryDetermineItemType(TypeInfo collectionTypeInfo)
        {
            var itemType = typeof(object);
            foreach (var inter in collectionTypeInfo.ImplementedInterfaces)
            {
                var interTypeInfo = inter.GetTypeInfo();
                if (interTypeInfo.IsGenericType &&
                    interTypeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    itemType = interTypeInfo.GenericTypeArguments[0];
                    break;
                }
            }

            return itemType;
        }

        protected override object DeseralizeSafe(BinaryReader reader)
        {
            var result = Activator.CreateInstance(this.ObjectType);
            var list = result as IList;
            if (list != null)
            {
                var itemsCount = reader.ReadInt32();
                for (int i = 0; i < itemsCount; i++)
                {
                    var item = this.ItemSerializer.Deserialzie(reader);
                    list.Add(item);
                    //var data = this.ReadObject(reader, itemType);
                    //list.Add(data);
                }
            }
            else
            {
                throw new InvalidOperationException("Can't deserialize collection");
            }

            return list;
        }

        protected override void SerializeSafe(BinaryWriter writer, object data)
        {
            var list = data as IList;
            if (list != null)
            {
                writer.Write(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    this.ItemSerializer.Serialize(writer, list[i]);
                }
            }
            else
            {
                throw new InvalidOperationException("Can't serialize collection");
            }
        }
    }
}
