using UnityEngine;

namespace MUXIGame.Serializer{
    public class TestBehaviour : MonoBehaviour{
        private void Start(){
            var testData = new TestData {
                test = "456"
            };
            testData.testData = new TestData2 {
                name = "123", self = gameObject
            };
            var valueTuple = PolymorphismSerializer.Serialize(testData);
            Debug.Log(valueTuple.Item1);
            valueTuple.Item2.ForEach(Debug.Log);
            var deserialize = PolymorphismSerializer.Deserialize<TestData>(valueTuple.Item1, valueTuple.Item2);
            Debug.Log(PolymorphismSerializer.Serialize(deserialize).Item1);
        }
        
    }
}