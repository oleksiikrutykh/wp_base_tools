namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;

    public class BinaryFormatter
    {
        private Type objectType;

        public BinaryFormatter(Type objectType)
        {
            this.objectType = objectType;
        }

        public void Seriazlie(Stream stream, object data)
        {
            var writer = new BinaryWriter(stream);
            var serializer = TypeStorage.GetForType(objectType);
            serializer.Serialize(writer, data);
            writer.Flush();
        }

        public object Deserialze(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var serialzier = TypeStorage.GetForType(this.objectType);
            return serialzier.Deserialzie(reader);
        }
    }

    internal class SerializingProperty
    {
        private AbstractTypeSerializer propertyWriter;

        public SerializingProperty(string name, PropertyInfo info)
        {
            this.PropertyInfo = info;
            this.Name = name;
        }

        public PropertyInfo PropertyInfo { get; private set; }

        public object DefaultValue { get; set; }

        public string Name { get; set; }

        public AbstractTypeSerializer PropertyWriter
        {
            get
            {
                if (this.propertyWriter == null)
                {
                    this.propertyWriter = TypeStorage.GetForType(this.PropertyInfo.PropertyType);
                }

                return propertyWriter;
            }
        }
    }
}
