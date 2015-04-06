namespace BaseTools.Core.Serialization.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class ArrayTypeSerializer : TypeCheckerSerializer
    {
        private Type itemType;

        private AbstractTypeSerializer itemSerializer;


        public AbstractTypeSerializer ItemSeriazlier
        {
            get
            {
                if (this.itemSerializer == null)
                {
                    this.itemSerializer = TypeStorage.GetForType(itemType);
                }

                return this.itemSerializer;
            }
        }

        public ArrayTypeSerializer(Type objectType, Type itemType)
            : base(objectType)
        { 
            this.itemType = itemType;
        }

        public static ArrayTypeSerializer TryCreate(Type type, TypeInfo typeInfo)
        {
            ArrayTypeSerializer result = null;
            if (type.IsArray)
            { 
                var itemType = type.GetElementType();
                result = new ArrayTypeSerializer(type, itemType);
            }

            return result;
        }

        protected override object DeseralizeSafe(System.IO.BinaryReader reader)
        {
            var arraySize = reader.ReadInt32();
            var array = Array.CreateInstance(this.itemType, arraySize);
            for (int i = 0; i < array.Length; i++)
            {
                var item = this.ItemSeriazlier.Deserialzie(reader);
                array.SetValue(item, i);
            }

            return array;
        }

        protected override void SerializeSafe(System.IO.BinaryWriter writer, object data)
        {
            var array = (Array)data;
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                var item = array.GetValue(i);
                this.ItemSeriazlier.Serialize(writer, item);
            }
        }
    }
}
