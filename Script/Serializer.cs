using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace MUXIGame.Serializer{
    public class Serializer{
        private readonly List<string> _typeDefinition;
        private readonly List<Object> _unityReference;
        public Serializer(){
            _typeDefinition = new List<string>();
            _unityReference = new List<Object>();
        }

        private int RegisterReference(Object unityObject){
            var index = _unityReference.IndexOf(unityObject);
            if (index != -1) return index;
            _unityReference.Add(unityObject);
            return _unityReference.Count - 1;
        }

        private int RegisterTypeDefinition(Type type){
            var index = _typeDefinition.IndexOf(type.FullName);
            if (index != -1) return index;
            _typeDefinition.Add(type.FullName);
            return _typeDefinition.Count - 1;
        }

        //Object
        internal string Serialize_Internal(object obj){
            if (obj is Array array){
                var jsonArray = new string[array.Length];
                for (var i = 0; i < array.Length; i++) jsonArray[i] = Reflect(array.GetValue(i));
                return CombineToJsonArray(jsonArray);
            }

            return Reflect(obj);
        }

        public (string, List<Object>) Serialize(object obj){
            _typeDefinition.Clear();
            _unityReference.Clear();
            return CombinePolymorphismInfo(Serialize_Internal(obj));
        }

        //Object
        private string Reflect(object obj){
            var objType = obj.GetType();
            var fieldInfos = objType.GetFields();
            var jsonField = new List<string>();
            if (obj is Object unityObject) return CombineToJsonObject(CombineToJsonField("$unityReference", RegisterReference(unityObject).ToString()));
            foreach (var fieldInfo in fieldInfos){
                object fieldValue;
                if (typeof(IField).IsAssignableFrom(fieldInfo.FieldType) && (fieldValue = fieldInfo.GetValue(obj)) != null) jsonField.Add(CombineToJsonField(fieldInfo.Name, (fieldValue as IField)?.GetValue(this)));
            }

            var properties = objType.GetProperties();
            foreach (var propertyInfo in properties){
                object fieldValue;
                if (typeof(IField).IsAssignableFrom(propertyInfo.PropertyType) && (fieldValue = propertyInfo.GetValue(obj)) != null) jsonField.Add(CombineToJsonField(propertyInfo.Name, (fieldValue as IField)?.GetValue(this)));
            }

            jsonField.Add(CombineToJsonField("$type", RegisterTypeDefinition(objType)));
            return CombineToJsonObject(jsonField.ToArray());
        }

        private string CombineToJsonField(string name, object value){ return $"{name}:{value}"; }
        private string CombineToJsonObject(params string[] str){ return "{" + string.Join(",", str) + "}"; }
        private string CombineToJsonArray(params string[] str){ return "["  + string.Join(",", str) + "]"; }
        private (string, List<Object>) CombinePolymorphismInfo(string json){ return (CombineToJsonObject(CombineToJsonField("data", json), CombineToJsonField("typeDefinition", CombineToJsonArray(_typeDefinition.Select(x=>$"\"{x}\"").ToArray()))), _unityReference); }
    }
}