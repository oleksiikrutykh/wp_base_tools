namespace BaseTools.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class LazyLoadSerializer
    {
        private AbstractTypeSerializer serializer;

        public LazyLoadSerializer(Type objectType)
        {
            this.ObjectType = objectType;
        }

        public Type ObjectType { get; private set; }

        public AbstractTypeSerializer Serializer
        {
            get
            {
                if (this.serializer == null)
                {
                    this.serializer = TypeStorage.GetForType(this.ObjectType);
                }

                return this.serializer;
            }
        }
    }
}
