namespace BinarySerializing.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal abstract class TypeCheckerSerializer : AbstractTypeSerializer
    {
        private const string NullValue = " ";

        public TypeCheckerSerializer(Type objectType)
        {
            this.ObjectType = objectType;
        }

        protected Type ObjectType { get; private set; }

        public override sealed object Deserialzie(BinaryReader reader)
        {
            object result = null;
            var typeName = reader.ReadString();
            if (String.IsNullOrEmpty(typeName))
            {
                result = this.DeseralizeSafe(reader);
            }
            else
            {
                if (typeName != NullValue)
                {
                    var realType = Type.GetType(typeName);
                    var engine = TypeStorage.GetForType(realType);
                    result = engine.Deserialzie(reader);
                }
                else
                {
                    // read null value
                    result = null;
                }
            }

            return result;
        }

        public override sealed void Serialize(BinaryWriter writer, object data)
        {
            if (data != null)
            {
                var dataType = data.GetType();
                string typeName = string.Empty;
                if (this.ObjectType != dataType)
                {
                    typeName = dataType.AssemblyQualifiedName;
                }

                writer.Write(typeName);

                if (typeName == string.Empty)
                {
                    this.SerializeSafe(writer, data);
                }
                else
                {
                    var serialzier = TypeStorage.GetForType(dataType);
                    serialzier.Serialize(writer, data);
                }
            }
            else
            {
                // Add null check symbol.
                writer.Write(NullValue);
            }
        }

        protected abstract object DeseralizeSafe(BinaryReader reader);

        protected abstract void SerializeSafe(BinaryWriter writer, object data);
    }
}
