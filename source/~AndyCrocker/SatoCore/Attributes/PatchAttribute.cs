/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace SatoCore.Attributes
{
    /// <summary>Specifies information about a Harmony patch.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PatchAttribute : Attribute
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The type of patch the method is.</summary>
        public PatchType PatchType { get; }

        /// <summary>The method to patch.</summary>
        public MethodInfo OriginalMethod { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="patchType">The type of patch the method is.</param>
        /// <param name="type">The type that contains the method to patch.</param>
        /// <param name="methodName">The name of the method to patch.</param>
        /// <param name="parameterTypes">The types of parameters of the method to patch.</param>
        public PatchAttribute(PatchType patchType, Type type, string methodName, Type[] parameterTypes = null)
        {
            parameterTypes ??= new Type[0];

            PatchType = patchType;
            OriginalMethod = AccessTools.Method(type, methodName, parameterTypes) ?? throw new ArgumentException($"Couldn't find method '{type.FullName}.{methodName}({string.Join(", ", parameterTypes.Select(parameterType => parameterType.Name))})'");
        }
    }
}
