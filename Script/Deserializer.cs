using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;

namespace MUXIGame.Serializer{
    public class Deserializer{
        private Type[] _typeDefinition;
        private List<Object> _unityReference;
        internal Type GetType(int index) => _typeDefinition[index];
        internal Object GetReference(int index) => _unityReference[index];
        public T Deserialize<T>(string json, List<Object> unityReference) where T : class, new(){
            _unityReference = unityReference;
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            if (jObject == null) return null;
            _typeDefinition=jObject["typeDefinition"]?.Values<string>().Select(x=>Type.GetType(x)).ToArray();
            return Visit(jObject["data"] as JObject) as T;
        }

        private object Visit(JObject data){
            var type = _typeDefinition[data["$type"]?.Value<int>() ?? throw new Exception("Can not find type")];
            var instance = Activator.CreateInstance(type);
            foreach (var jToken in data){
                var fieldInfo = type.GetField(jToken.Key);
                if(fieldInfo ==null ||!typeof(IField).IsAssignableFrom(fieldInfo.FieldType)) continue;
                var field = Activator.CreateInstance(fieldInfo.FieldType);
                var setMethod = fieldInfo.FieldType.GetMethod("SetValue");
                if (jToken.Value is JObject jObject){
                    if (!jObject.ContainsKey("$unityReference")){
                        fieldInfo.FieldType.GetField("value")?.SetValue(field,Visit(jObject));
                        fieldInfo.SetValue(instance,field);
                        continue;
                    }
                }
                setMethod?.Invoke(field,new []{(object)jToken.Value,this});
                fieldInfo.SetValue(instance,field);
            }
            return instance;
        }
    }
}