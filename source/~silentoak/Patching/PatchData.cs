using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using SilentOak.Patching.Extensions;

namespace SilentOak.Patching
{
    public class PatchData
    {
        /*************
         * Properties
         *************/

        /// <summary>The method(s) to patch.</summary>
        public IEnumerable<MethodBase> MethodsToPatch { get; private set; }

        /// <summary>The exception that occured during initialization</summary>
        public Exception Exception { get; private set; }


        /*****************
         * Public methods
         *****************/

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="originalMethods">Original methods.</param>
        public PatchData(IEnumerable<MethodBase> originalMethods)
        {
            MethodsToPatch = originalMethods;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="originalMethod">Original method.</param>
        public PatchData(MethodBase originalMethod)
        : this(new MethodBase[] { originalMethod })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="types">Types.</param>
        /// <param name="originalMethod">Original method.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            IEnumerable<Type> types,
            string originalMethod,
            params Type[] originalMethodParams
        )
        {
            CaptureExceptions(() =>
            {
                Initialize(types, originalMethod, originalMethodParams);
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            Type type,
            string originalMethodName,
            params Type[] originalMethodParams
        ) : this(new Type[] { type }, originalMethodName, originalMethodParams)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="typeNames">Type names.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            Assembly assembly,
            IEnumerable<string> typeNames,
            string originalMethodName,
            params Type[] originalMethodParams
        )
        {
            CaptureExceptions(() =>
            {
                Initialize(assembly, typeNames, originalMethodName, originalMethodParams);
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="typeName">Type name.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            Assembly assembly,
            string typeName,
            string originalMethodName,
            params Type[] originalMethodParams
        ) : this(assembly, new string[] { typeName }, originalMethodName, originalMethodParams)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="assemblyVersion">Assembly version.</param>
        /// <param name="typeNames">Type names.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            string assemblyName,
            string assemblyVersion,
            IEnumerable<string> typeNames,
            string originalMethodName,
            params Type[] originalMethodParams
        )
        {
            CaptureExceptions(() =>
            {
                Initialize(assemblyName, assemblyVersion, typeNames, originalMethodName, originalMethodParams);
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SilentOak.Patching.PatchData"/> class.
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="assemblyVersion">Assembly version.</param>
        /// <param name="typeName">Type name.</param>
        /// <param name="originalMethod">Original method.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        public PatchData(
            string assemblyName,
            string assemblyVersion,
            string typeName,
            string originalMethod,
            params Type[] originalMethodParams
        ) : this(assemblyName, assemblyVersion, new string[] { typeName }, originalMethod, originalMethodParams)
        {
        }


        /*******************
         * Internal methods
         *******************/

        /// <summary>
        /// Initialize with the specified types, originalMethod and originalMethodParams.
        /// </summary>
        /// <param name="types">Types.</param>
        /// <param name="originalMethod">Original method.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        internal void Initialize(
            IEnumerable<Type> types,
            string originalMethod,
            params Type[] originalMethodParams
        )
        {
            MethodsToPatch = CalculateMethods(types, originalMethod, originalMethodParams);
        }

        /// <summary>
        /// Initialize with the specified assembly, typeNames, originalMethodName and originalMethodParams.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="typeNames">Type names.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        internal void Initialize(
            Assembly assembly,
            IEnumerable<string> typeNames,
            string originalMethodName,
            params Type[] originalMethodParams
        )
        {
            Initialize(typeNames.Select(tn => assembly.GetType(tn, throwOnError: true)), originalMethodName, originalMethodParams);
        }

        /// <summary>
        /// Initialize with the specified assemblyName, assemblyVersion, typeNames, originalMethodName and originalMethodParams.
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="assemblyVersion">Assembly version.</param>
        /// <param name="typeNames">Type names.</param>
        /// <param name="originalMethodName">Original method name.</param>
        /// <param name="originalMethodParams">Original method parameters.</param>
        internal void Initialize(
            string assemblyName,
            string assemblyVersion,
            IEnumerable<string> typeNames,
            string originalMethodName,
            params Type[] originalMethodParams
        )
        {
            Initialize(GetAssemblyByNameAndVersion(assemblyName, assemblyVersion), typeNames, originalMethodName, originalMethodParams);
        }

        /// <summary>
        /// Captures the exceptions.
        /// </summary>
        /// <param name="action">Action.</param>
        internal void CaptureExceptions(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }


        /// <summary>Retrieves the methods of the given name from the given types.</summary>
        /// <returns>The methods.</returns>
        internal static IEnumerable<MethodBase> CalculateMethods(IEnumerable<Type> types, string methodName, params Type[] methodParams)
        {
            foreach (Type type in types)
            {
                MethodBase method;
                if (methodName == ".ctor")
                {
                    method = AccessTools.Constructor(type, methodParams);
                }
                else
                {
                    method = AccessTools.Method(type, methodName, methodParams);
                }

                if (method == null)
                {
                    throw new MissingMethodException(type.FullName, methodName);
                }

                yield return method;
            }
        }


        /// <summary>Gets the loaded assembly with the given name and version.</summary>
        /// <returns>The assembly.</returns>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="assemblyVersion">Assembly version.</param>
        internal static Assembly GetAssemblyByNameAndVersion(string assemblyName, string assemblyVersion)
        {
            IEnumerable<Assembly> loadedMatchingAssemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == assemblyName);

            if (!loadedMatchingAssemblies.Any())
            {
                throw new DllNotFoundException($"Could not find {assemblyName}.");
            }

            if (loadedMatchingAssemblies.Count() > 1)
            {
                throw new AmbiguousMatchException($"Found multiple {assemblyName}.");
            }

            Assembly candidateAssembly = loadedMatchingAssemblies.First();
            Version candidateAssemblyVersion = candidateAssembly.GetName().Version;
            if (!candidateAssemblyVersion.Match(assemblyVersion))
            {
                throw new DllNotFoundException($"Found version {candidateAssemblyVersion.ToString()}, required {assemblyVersion}");
            }

            return candidateAssembly;
        }
    }
}