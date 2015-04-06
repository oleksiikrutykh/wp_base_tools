namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    public class BinarySerializer : ISerializer
    {
        public void Serialize<T>(System.IO.Stream stream, T storedObj)
        {
            var serializer = new BinaryFormatter(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                serializer.Seriazlie(memoryStream, storedObj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(stream);
            }
        }

        public T Deserialize<T>(System.IO.Stream stream)
        {
            var serializer = new BinaryFormatter(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialze(memoryStream);
            }
        }
    }
}
