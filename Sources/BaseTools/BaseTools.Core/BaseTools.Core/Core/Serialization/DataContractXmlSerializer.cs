namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document.
    /// </summary>
    public class DataContractXmlSerializer : ISerializer
    {
        private List<Type> knownTypes = new List<Type>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<Type> KnownTypes
        {
            get
            {
                return this.knownTypes;
            }
        }

        public void Serialize<T>(System.IO.Stream stream, T storedObj)
        {
            var serializer = new DataContractSerializer(typeof(T), this.knownTypes);
            serializer.WriteObject(stream, storedObj);
        }

        public T Deserialize<T>(System.IO.Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T), this.knownTypes);
            return (T)serializer.ReadObject(stream);
        }
    }
}
