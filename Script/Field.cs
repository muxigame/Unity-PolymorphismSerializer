using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;

namespace MUXIGame.Serializer{
    public interface IField{
        string GetValue(Serializer polymorphismSerializer);
        void SetValue(JToken jsonValue, Deserializer polymorphismSerializer);
    }

    public sealed class Field<T> : IField{
        public T value;

        public string GetValue(Serializer polymorphismSerializer){
            if (value == null) return "null";
            switch (value){
                case char:
                case byte:
                case short:
                case ushort:
                case int:
                case uint:
                case long:
                case ulong:
                    return value.ToString();
                case float:
                case double:
                    return value.ToString();
                case bool:
                    return value.ToString();
                case string:
                    return $"\"{value.ToString()}\"";
                case Object:
                case Array:
                case { }:
                    return polymorphismSerializer.Serialize_Internal(value);
                default:
                    return polymorphismSerializer.Serialize_Internal(value);
            }
        }

        public void SetValue(JToken jsonValue, Deserializer polymorphismSerializer){
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string)) value = (T) Convert.ChangeType(jsonValue, typeof(T));
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType != null && underlyingType.IsPrimitive || underlyingType == typeof(string)) value = (T) Convert.ChangeType(jsonValue, typeof(T));
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && jsonValue is JArray) value = JsonConvert.DeserializeObject<T>(jsonValue.ToString());
            if (typeof(T).IsSubclassOf(typeof(Object))) value = (T)(object)polymorphismSerializer.GetReference(jsonValue["$unityReference"]?.Value<int>() ?? throw new Exception("Can not find reference"));
        }
    
    
        public static implicit operator Field<T>(T value){
            return new Field<T> {
                value = value
            };
        }
    }
}