using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SilentOak.Patching.Exceptions;
using Harmony;

namespace SilentOak.Patching.Tests
{
    /// <summary>
    /// Patch manager tests.
    /// </summary>
    [TestFixture]
    public class PatchManagerTests
    {
        /// <summary>
        /// Ensures that <see cref="PatchData.GetAssemblyByNameAndVersion(string, string)"/> returns this assembly when given its name. 
        /// </summary>
        [TestCase]
        public void Test_GetAssemblyByName_ThisAssembly()
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            Assembly foundAssembly = PatchData.GetAssemblyByNameAndVersion(thisAssembly.GetName().Name, "*");
            Assert.AreEqual(thisAssembly, foundAssembly);
        }

        /// <summary>
        /// Ensures that <see cref="PatchData.GetAssemblyByNameAndVersion(string, string)"/> throws on non-existent assembly.
        /// </summary>
        [TestCase]
        public void Test_GetAssemblyByName_NoAssembly()
        {
            Assert.Throws<DllNotFoundException>(() => PatchData.GetAssemblyByNameAndVersion("null", "*"));
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.GetPatchData(Type)"/> throws on invalid patch class.
        /// </summary>
        [TestCase]
        public void Test_GetPatchData_NoPatchData()
        {
            Assert.Throws<MissingAttributeException>(() => PatchManager.GetPatchData(typeof(int)));
        }


        public static class PatchTest1
        {
            public static readonly PatchData PatchData = new PatchData(
                assemblyName: "PatchTest",
                assemblyVersion: "1.0.0",
                typeName: "PatchTest.Test",
                originalMethod: "NoTest"
            );
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.GetPatchData(Type)"/> retrieves correctly the PatchData for a patch class.
        /// </summary>
        [TestCase]
        public void Test_GetPatchData_HadPatchData()
        {
            PatchData testPatchData = new PatchData(
                assemblyName: "PatchTest",
                assemblyVersion: "1.0.0",
                typeName: "PatchTest.Test",
                originalMethod: "NoTest"
                );

            Assert.IsNotNull(PatchManager.GetPatchData(typeof(PatchTest1)));
        }


        public static class PatchTest2
        {
            public static readonly PatchData PatchData = new PatchData(
                assemblyName: "NoAssembly",
                assemblyVersion: "*",
                typeName: "Anything",
                originalMethod: "Anything"
            );
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.Apply(Type)"/> throws when the assembly is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_DllMissing()
        {
            Assert.Throws<DllNotFoundException>(() =>
                PatchManager.Apply(typeof(PatchTest2))
            );
        }


        public static class PatchTest3
        {
            public static readonly PatchData PatchData = new PatchData(
                assemblyName: typeof(PatchTest3).Assembly.GetName().Name,
                assemblyVersion: "0.1.0",
                typeName: "Anything",
                originalMethod: "Anything"
            );
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.Apply(Type)"/> throws when the assembly has the wrong version.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_DllWrongVersion()
        {
            Assert.Throws<DllNotFoundException>(() => PatchManager.Apply(typeof(PatchTest3)));
        }


        public static class PatchTest4
        {
            public static readonly PatchData PatchData = new PatchData(
                assembly: typeof(PatchTest4).Assembly,
                typeName: "NoType",
                originalMethodName: "Anything"
            );
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.Apply(Type)"/> throws when the type is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_TypeMissing()
        {
            Assert.Throws<TypeLoadException>(() => PatchManager.Apply(typeof(PatchTest4)));
        }


        public static class PatchTest5
        {
            public static readonly PatchData PatchData = new PatchData(
                type: typeof(PatchTest5),
                originalMethodName: "NoMethod"
            );
        }

        /// <summary>
        /// Ensures that <see cref="PatchManager.Apply(Type)"/> throws when the method is not found.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_MethodMissing()
        {
            Assert.Throws<MissingMethodException>(() => PatchManager.Apply(typeof(PatchTest5)));
        }


        public static class PatchTest6
        {
            public static readonly PatchData PatchData = new PatchData(
                type: typeof(PatchTest6),
                originalMethodName: "Test",
                originalMethodParams: new Type[] { typeof(int) }
            );

            public static void Test(int dummy)
            {
                return;
            }
        }

        /// <summary>
        /// Ensures that <see cref="PatchData.CalculateMethods"/> successfully retrieves the correct method.
        /// </summary>
        [TestCase]
        public void Test_CalculateMethod_Success()
        {
            PatchData patchData = PatchManager.GetPatchData(typeof(PatchTest6));

            MethodInfo testMethod = AccessTools.Method(typeof(PatchTest6), "Test");

            Assert.AreEqual(testMethod, patchData.MethodsToPatch.SingleOrDefault());
        }
    }
}