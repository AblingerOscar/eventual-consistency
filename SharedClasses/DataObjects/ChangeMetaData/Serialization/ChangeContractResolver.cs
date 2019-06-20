using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SharedClasses.DataObjects.ChangeMetaData.Serialization
{
    internal class ChangeContractResolver<T>: DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(T).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend Converter is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
}