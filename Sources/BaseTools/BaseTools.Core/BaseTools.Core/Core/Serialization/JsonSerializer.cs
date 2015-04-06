namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// Serializes and deserializes an instance of a type into an json stream
    /// </summary>
    public class JsonSerializer : ISerializer
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

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), this.knownTypes);
            return (T)serializer.ReadObject(stream);
        }

        public void Serialize<T>(Stream stream, T storedObj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), this.knownTypes);
            serializer.WriteObject(stream, storedObj);
        }
    }
}
