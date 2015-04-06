namespace BaseTools.Core.Serialization.CustomTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.IO;
    using System.Runtime.Serialization;



    [System.Runtime.Serialization.DataContract]
    internal sealed class CustomTypeSerializer : TypeCheckerSerializer
    {
        private static Dictionary<Type, bool> isDataContractType = new Dictionary<Type, bool>();

        public static bool IsDataContractType(Type type)
        {
            var dataContractAttribute = type.GetTypeInfo().GetCustomAttribute(typeof(DataContractAttribute));
            return dataContractAttribute != null;
            //bool result = false;
            //var isExist = isDataContractType.TryGetValue(type, ou)
        }


        private List<MethodInfo> deserializingMethods = new List<MethodInfo>();
 
        private List<MethodInfo> deserializedMethods = new List<MethodInfo>();

        private List<MethodInfo> serializingMethods = new List<MethodInfo>();

        private List<MethodInfo> serializedMethods = new List<MethodInfo>();

        private List<SerializingProperty> StoredProperties;

        public CustomTypeSerializer(Type objectType) :
            base(objectType)
        {
            this.StoredProperties = new List<SerializingProperty>();
            var allRuntimeProperties = this.ObjectType.GetRuntimeProperties();
            foreach (var runtimeProperty in allRuntimeProperties)
            {
                // TODO: add check for datamember, throw informative exception.
                if (runtimeProperty.CanRead &&
                    runtimeProperty.CanWrite &&
                    runtimeProperty.GetMethod.IsPublic &&
                    runtimeProperty.SetMethod.IsPublic)
                {
                    string propertyName = null;
                    if (IsDataContractType(runtimeProperty.DeclaringType))
                    {
                        var dataMemberAttribute = (DataMemberAttribute)runtimeProperty.GetCustomAttribute(typeof(DataMemberAttribute));
                        if (dataMemberAttribute != null)
                        {
                            propertyName = dataMemberAttribute.Name;
                            if (string.IsNullOrEmpty(propertyName))
                            {
                                propertyName = runtimeProperty.Name;
                            }
                        }
                    }
                    else
                    {
                        propertyName = runtimeProperty.Name;
                    }

                    if (propertyName != null)
                    {
                        var property = new SerializingProperty(propertyName, runtimeProperty);
                        this.StoredProperties.Add(property);
                    }
                }
            }


            var allMethods = this.ObjectType.GetRuntimeMethods();
            foreach (var method in allMethods)
            {
                if (method.IsPublic)
                {
                    if (IsDataContractType(method.DeclaringType))
                    {
                        // TODO: add limits for calling
                        foreach (var attribute in method.CustomAttributes)
                        {
                            var attributeType = attribute.AttributeType;
                            if (attributeType == typeof(OnDeserializedAttribute))
                            {
                                this.deserializedMethods.Add(method);
                            }
                            else if (attributeType == typeof(OnDeserializingAttribute))
                            {
                                this.deserializingMethods.Add(method);
                            }
                            else if (attributeType == typeof(OnSerializedAttribute))
                            {
                                this.serializedMethods.Add(method);
                            }
                            else if (attributeType == typeof(OnSerializingAttribute))
                            {
                                this.serializingMethods.Add(method);
                            }
                        }
                    }
                }
            } 
        }

        public static CustomTypeSerializer TryCreate(Type type, TypeInfo typeInfo)
        {
            var result = new CustomTypeSerializer(type);
            return result;
        }

        protected sealed override object DeseralizeSafe(BinaryReader reader)
        {
            var result = Activator.CreateInstance(this.ObjectType);

            foreach (var method in this.deserializingMethods)
            {
                method.Invoke(result, new object[1] { null });
            }
            
            var propertyName = string.Empty;
            int startIndex = 0;
            do
            {
                propertyName = reader.ReadString();
                if (propertyName != string.Empty)
                {
                    
                    // TODO: improve first or default - use indexes search.
                    var property = IListHelpers.FirstOrDefault(this.StoredProperties, p => p.Name == propertyName, ref startIndex);
                    //var property = this.StoredProperties.FirstOrDefault(p => p.Name == propertyName);
                    var propertyValue = property.PropertyWriter.Deserialzie(reader);
                    property.PropertyInfo.SetValue(result, propertyValue);
                }
            }
            while (propertyName != string.Empty);

            foreach (var method in this.deserializedMethods)
            {
                method.Invoke(result, new object[1] { null });
            }

            return result;
        }

        protected sealed override void SerializeSafe(BinaryWriter writer, object data)
        {
            foreach (var method in this.serializingMethods)
            {
                method.Invoke(data, new object[1] { null });
            }


            foreach (var property in this.StoredProperties)
            {
                var itemValue = property.PropertyInfo.GetValue(data);
                //if (!Object.Equals(itemValue, property.DefaultValue))
                {
                    writer.Write(property.Name);
                    property.PropertyWriter.Serialize(writer, itemValue);
                }
            }

            // Write empty propertyName to ensure object ended.
            writer.Write(string.Empty);

            foreach (var method in this.serializedMethods)
            {
                method.Invoke(data, new object[1] { null });
            }
        }
    }

    public static class IListHelpers
    {
        public static T FirstOrDefault<T>(IList<T> list, Func<T, bool> selector, ref int startIndex)
        {
            T result = default(T);
            bool isItemFounded = false;
            int newItemIndex = -1;
            for (int i = startIndex; i < list.Count; i++)
            {
                var currentItem = list[i];
                isItemFounded = selector.Invoke(currentItem);
                if (isItemFounded)
                {
                    newItemIndex = i;
                    result = currentItem;
                    break;
                }
            }

            if (!isItemFounded)
            {
                for (int i = 0; i < startIndex; i++)
                {
                    var currentItem = list[i];
                    isItemFounded = selector.Invoke(currentItem);
                    if (isItemFounded)
                    {
                        newItemIndex = i;
                        result = currentItem;
                        break;
                    }
                }
            }

            if (isItemFounded)
            {
                startIndex = newItemIndex + 1;
                if (startIndex >= list.Count)
                {
                    startIndex = 0;
                }
            }

            return result;
        }
    }
}
