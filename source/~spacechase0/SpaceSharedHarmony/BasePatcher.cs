/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using Harmony;
using StardewModdingAPI;

namespace Spacechase.Shared.Harmony
{
    /// <summary>Provides base implementation logic for <see cref="IPatcher"/> instances.</summary>
    internal abstract class BasePatcher : IPatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public abstract void Apply(HarmonyInstance harmony, IMonitor monitor);


        /*********
        ** Protected methods
        *********/
        /// <summary>Get a method and assert that it was found.</summary>
        /// <typeparam name="TTarget">The type containing the method.</typeparam>
        /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
        protected ConstructorInfo RequireConstructor<TTarget>(params Type[] parameters)
        {
            return PatchHelper.RequireConstructor<TTarget>(parameters);
        }

        /// <summary>Get a method and assert that it was found.</summary>
        /// <typeparam name="TTarget">The type containing the method.</typeparam>
        /// <param name="name">The method name.</param>
        /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
        /// <param name="generics">The method generic types, or <c>null</c> if it's not generic.</param>
        protected MethodInfo RequireMethod<TTarget>(string name, Type[] parameters = null, Type[] generics = null)
        {
            return PatchHelper.RequireMethod<TTarget>(name, parameters, generics);
        }

        /// <summary>Get a Harmony patch method on the current patcher instance.</summary>
        /// <param name="name">The method name.</param>
        /// <param name="priority">The patch priority to apply, usually specified using Harmony's <see cref="Priority"/> enum, or <c>null</c> to keep the default value.</param>
        protected HarmonyMethod GetHarmonyMethod(string name, int? priority = null)
        {
            var method = new HarmonyMethod(
                AccessTools.Method(this.GetType(), name)
                ?? throw new InvalidOperationException($"Can't find patcher method {PatchHelper.GetMethodString(this.GetType(), name)}.")
            );

            if (priority.HasValue)
                method.prioritiy = priority.Value;

            return method;
        }
    }
}
