using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharedClasses.DataObjects.ChangeMetaData.Serialization
{
    internal class ChangeConverter : JsonConverter
    {
        private const string CHANGE_DO_NAMESPACE = nameof(SharedClasses) + "." + nameof(SharedClasses.DataObjects) + "." + nameof(SharedClasses.DataObjects.ChangeMetaData);

        private static readonly ISet<Type> knownChangeTypes;
        private static readonly IDictionary<FileChangeMetaData.ChangeType, Type> enumToType;

        private static readonly JsonSerializerSettings DefaultJsonSeriializerSettings = new JsonSerializerSettings() { ContractResolver = new ChangeContractResolver<FileChangeMetaData>() };

        static ChangeConverter()
        {
            var assembly = Assembly.GetExecutingAssembly();

            enumToType = assembly
                .GetTypes()
                .Where(t => string.Equals(t.Namespace, CHANGE_DO_NAMESPACE, StringComparison.Ordinal))
                .ToDictionary(t => t, t => t.GetCustomAttributes(typeof(FileChangeTypeAttribute), true).FirstOrDefault())
                .Where(kvp => kvp.Value != default)
                .ToDictionary(kvp => (kvp.Value as FileChangeTypeAttribute).ChangeType, kvp => kvp.Key);

            knownChangeTypes = enumToType.Values.ToHashSet();
        }
        
        public override bool CanConvert(Type objectType)
        {
            return knownChangeTypes.Contains(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var changeType = jo[nameof(FileChangeMetaData.Type)].Value<int>();
            var concreteType = enumToType[(FileChangeMetaData.ChangeType)changeType];

            return JsonConvert.DeserializeObject(jo.ToString(), concreteType, DefaultJsonSeriializerSettings);
        }

        public override bool CanWrite {
            get {
                return false;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // Won't be called, becaues CanWrite is alwads false
        }
    }
}
