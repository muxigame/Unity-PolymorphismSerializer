using UnityEngine;

namespace MUXIGame.Serializer{
    public class TestData{
        public TestData(){}
        public Field<string> test;

        public Field<TestData> testData;
    }

    public class TestData2 : TestData{
        public TestData2(){}
        public Field<string> name;
        public Field<GameObject> self;
    }
}