namespace PhoneApp14.BinarySerialzier
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;

    public class BinarySerializer<T> : Serialzier<T>
    {
        public override void Serialize(System.IO.Stream stream, T data)
        {
            var writer = new BinaryWriter(stream);
            this.WriteObject(writer, data, typeof(T));
        }

        private void WriteObject(BinaryWriter writer, object data, Type expectedType)
        {
            var dataType = data.GetType();
            string typeName = string.Empty;
            if (dataType != expectedType)
            {
                typeName = dataType.AssemblyQualifiedName;
            }

            writer.Write(typeName);

            if (dataType == typeof(int))
            {
                var intValue = (int)data;
                writer.Write(intValue);
            }
            else if (dataType == typeof(string))
            {
                var stringValue = (string)data;
                writer.Write(stringValue);
            }
            else if (data is IList)
            {
                var list = (IList)data;
                writer.Write(list.Count);
                var itemType = this.DetermineListItemType(dataType);
                for (int i = 0; i < list.Count; i++)
                {
                    this.WriteObject(writer, list[i], itemType);
                }
            }
            else
            {
                var propertyList = new List<object>();
                var properties = dataType.GetRuntimeProperties();
                //writer.Write(properties.Length);
                foreach (var property in properties)
                {
                    writer.Write(property.Name);
                    var propertyValue = property.GetValue(data);
                    this.WriteObject(writer, propertyValue, property.PropertyType);
                }
            }
        }

        private Type DetermineListItemType(Type collectionType)
        {
            Type itemType = typeof(object);
            foreach (Type interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition()
                    == typeof(IList<>))
                {
                    itemType = interfaceType.GetGenericArguments()[0];
                    break;
                }
            }

            return itemType;
        }

        public override T Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            return (T)this.ReadObject(reader, typeof(T));
        }

        private object ReadObject(BinaryReader reader, Type expectedType)
        {
            object result = null;
            var objectType = expectedType;
            var realTypeName = reader.ReadString();
            if (!string.IsNullOrEmpty(realTypeName))
            {
                objectType = Type.GetType(realTypeName);
            }

            if (objectType == typeof(int))
            {
                result = reader.ReadInt32();
            }
            else if (objectType == typeof(string))
            {
                result = reader.ReadString();
            }
            else
            {
                result = Activator.CreateInstance(objectType);
                if (result is IList)
                {
                    var list = (IList)result;
                    var itemType = this.DetermineListItemType(objectType);
                    var itemsCount = reader.ReadInt32();
                    for (int i = 0; i < itemsCount; i++)
                    {
                        var data = this.ReadObject(reader, itemType);
                        list.Add(data);
                    }
                }
                else
                {
                    var properties = objectType.GetRuntimeProperties();
                    foreach (var property in properties)
                    {
                        var propertyName = reader.ReadString();
                        var propertyValue = this.ReadObject(reader, property.PropertyType);
                        property.SetValue(result, propertyValue);
                    }
                }
            }

            return result;
        }
    }

}
