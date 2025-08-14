using System;
using System.Collections;
using System.Globalization;
using com.IvanMurzak.ReflectorNet;
using com.IvanMurzak.ReflectorNet.Utils;
using com.IvanMurzak.Unity.MCP.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.Editor.Tests
{
    public class TestJsonSerialize : BaseTest
    {
        static void ValidateType<T>(T sourceValue)
        {
            var serializedValue = JsonUtils.ToJson(sourceValue);
            var deserializedValue = JsonUtils.Deserialize<T>(serializedValue);

            var areEqual = Reflector.Instance.AreEqual(sourceValue, deserializedValue);
            Assert.IsTrue(areEqual, $"Serialized and deserialized values do not match for type '{typeof(T).GetTypeName(pretty: true)}'");
        }

        [UnityTest]
        public IEnumerator Primitives()
        {
            ValidateType(100);
            ValidateType(0.23f);
            ValidateType(true);
            ValidateType("hello world");
            ValidateType(CultureTypes.SpecificCultures); // enum

            yield return null;
        }

        [UnityTest]
        public IEnumerator Classes()
        {
            var go = new UnityEngine.GameObject("TestObject");
            ValidateType(go.ToObjectRef());

            yield return null;
        }

        [UnityTest]
        public IEnumerator Structs()
        {
            ValidateType(DateTime.Now);
            ValidateType(TimeSpan.FromSeconds(10));

            yield return null;
        }

        [UnityTest]
        public IEnumerator UnityStructs()
        {
            ValidateType(new UnityEngine.Color32(1, 50, 33, 255));
            ValidateType(UnityEngine.Color.cyan);
            ValidateType(UnityEngine.Vector3.up);
            ValidateType(UnityEngine.Vector3Int.up);
            ValidateType(UnityEngine.Vector2.up);
            ValidateType(UnityEngine.Vector2Int.up);
            ValidateType(UnityEngine.Quaternion.identity);
            ValidateType(UnityEngine.Matrix4x4.identity);

            yield return null;
        }

        [UnityTest]
        public IEnumerator UnityComponent()
        {
            var go = new UnityEngine.GameObject("TestObject");
            ValidateType(go.transform.ToObjectRef());
            ValidateType(go.AddComponent<UnityEngine.Rigidbody>().ToObjectRef());
            ValidateType(go.AddComponent<UnityEngine.SpriteRenderer>().ToObjectRef());
            ValidateType(go.AddComponent<UnityEngine.MeshRenderer>().ToObjectRef());

            yield return null;
        }

        [UnityTest]
        public IEnumerator UnityAsset()
        {
            ValidateType(new UnityEngine.Animation().ToObjectRef());
            ValidateType(new UnityEngine.Material(UnityEngine.Shader.Find("Standard")).ToObjectRef());

            yield return null;
        }
    }
}