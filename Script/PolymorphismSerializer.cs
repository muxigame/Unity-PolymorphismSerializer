using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MUXIGame.Serializer{
    [Serializable]
    public class PolymorphismSerializer{
        private PolymorphismSerializer(){ }
        public static Serializer Serializer = new Serializer();
        public static Deserializer Deserializer = new Deserializer();
        public static (string, List<Object>) Serialize(object obj){
            return Serializer.Serialize(obj);
        }
        public static T Deserialize<T>(string json, List<Object> unityReference) where T : class, new(){
            return Deserializer.Deserialize<T>(json,unityReference);
        }
    }
}