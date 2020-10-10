/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/funny-snek/anticheat-and-servercode
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;
using Harmony;

namespace FunnySnek.AntiCheat.Server.Framework
{
    //Remember Prefix/Postfix should be public and static! Do not use lambdas
    internal abstract class Patch
    {
        /*********
        ** Properties
        *********/
        protected abstract PatchDescriptor GetPatchDescriptor();


        /*********
        ** Public methods
        *********/
        public static void PatchAll(string id)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(id);

            var types = (
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where type.IsClass && type.BaseType == typeof(Patch)
                select type
            );
            foreach (Type type in types)
                ((Patch)Activator.CreateInstance(type)).ApplyPatch(harmonyInstance);
        }


        /*********
        ** Private methods
        *********/
        private void ApplyPatch(HarmonyInstance harmonyInstance)
        {
            var patchDescriptor = this.GetPatchDescriptor();

            MethodBase targetMethod = string.IsNullOrEmpty(patchDescriptor.TargetMethodName)
                ? (MethodBase)patchDescriptor.TargetType.GetConstructor(patchDescriptor.TargetMethodArguments ?? new Type[0])
                : (patchDescriptor.TargetMethodArguments != null
                    ? patchDescriptor.TargetType.GetMethod(patchDescriptor.TargetMethodName, patchDescriptor.TargetMethodArguments)
                    : patchDescriptor.TargetType.GetMethod(patchDescriptor.TargetMethodName, (BindingFlags)62)
                );

            harmonyInstance.Patch(targetMethod, new HarmonyMethod(this.GetType().GetMethod("Prefix")), new HarmonyMethod(this.GetType().GetMethod("Postfix")));
        }
    }
}
