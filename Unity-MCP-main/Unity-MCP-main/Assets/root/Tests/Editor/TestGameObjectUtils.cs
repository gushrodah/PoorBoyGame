using System.Collections;
using com.IvanMurzak.Unity.MCP.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.Editor.Tests
{
    public partial class TestGameObjectUtils
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Debug.Log($"[{nameof(TestGameObjectUtils)}] SetUp");
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Debug.Log($"[{nameof(TestGameObjectUtils)}] TearDown");
            yield return null;
        }

        [UnityTest]
        public IEnumerator FindByPath()
        {
            var parentName = "root";
            var childName = "nestedGo";
            new GameObject(parentName).AddChild(childName);

            var prefixes = new[] { "", "/" };
            foreach (var prefix in prefixes)
            {
                Assert.IsNotNull(GameObjectUtils.FindByPath($"{prefix}{parentName}"), $"{prefix}{parentName} should not be null");
                Assert.IsNotNull(GameObjectUtils.FindByPath($"{prefix}{parentName}/{childName}"), $"{prefix}{parentName}/{childName} should not be null");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator GetPath()
        {
            var parentName = "root";
            var childName = "nestedGo";
            var child = new GameObject(parentName).AddChild(childName);

            Assert.AreEqual(child.GetPath(), $"{parentName}/{childName}",
                $"GameObject '{childName}' should have path '{parentName}/{childName}'");

            yield return null;
        }
    }
}