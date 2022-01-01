/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace BattleRoyale
{
    abstract class Patch
    {
        protected abstract PatchDescriptor GetPatchDescriptor();

        //Remember Prefix/Postfix should be public and static! Do not use lambdas

        protected class PatchDescriptor
        {
            public Type targetType;
            public string targetMethodName;
            public Type[] targetMethodArguments;

            /// <param name="targetType">Use typeof()</param>
            /// <param name="targetMethodName">Null if constructor is desired</param>
            /// <param name="targetMethodArguments">Null if no method abiguity</param>
            public PatchDescriptor(Type targetType, string targetMethodName, Type[] targetMethodArguments = null)
            {
                this.targetType = targetType;
                this.targetMethodName = targetMethodName;
                this.targetMethodArguments = targetMethodArguments;
            }
        }


        private void ApplyPatch(Harmony harmonyInstance)
        {
            var patchDescriptor = GetPatchDescriptor();

            MethodBase targetMethod = string.IsNullOrEmpty(patchDescriptor.targetMethodName) ?

                patchDescriptor.targetType.GetConstructor(patchDescriptor.targetMethodArguments ?? Array.Empty<Type>()) :

                targetMethod = patchDescriptor.targetMethodArguments != null ?
                    patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, patchDescriptor.targetMethodArguments)
                    : patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, ((BindingFlags)62));

            try
            {
                MethodInfo prefix = AccessTools.Method(this.GetType(), "Prefix");
                MethodInfo postfix = AccessTools.Method(this.GetType(), "Postfix");
                MethodInfo transpiler = AccessTools.Method(this.GetType(), "Transpiler") ?? AccessTools.Method(this.GetType(), "Transpile");

                HarmonyMethod hmPrefix = null;
                HarmonyMethod hmPostfix = null;
                HarmonyMethod hmTranspile = null;

                if (prefix != null) hmPrefix = new HarmonyMethod(prefix);
                if (postfix != null) hmPostfix = new HarmonyMethod(postfix);
                if (transpiler != null) hmTranspile = new HarmonyMethod(transpiler);

                harmonyInstance.Patch(targetMethod,
                    prefix: hmPrefix,
                    postfix: hmPostfix,
                    transpiler: hmTranspile);
            }
            catch (Exception e)
            {
                Console.WriteLine(this.GetType().FullName);
                Console.WriteLine(e.ToString());
            }
        }

        public static void PatchAll(string id)
        {
            Harmony harmonyInstance = new(id);

            foreach (Type type in (from type in Assembly.GetExecutingAssembly().GetTypes()
                                   where type.IsClass && type.BaseType == typeof(Patch)
                                   select type))
                ((Patch)Activator.CreateInstance(type)).ApplyPatch(harmonyInstance);

        }
    }
}
