/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehPers.Core.Api.Setup
{
    /// <summary>
    /// A service that applies Harmony patches.
    /// </summary>
    public abstract class Patcher : ISetup, IDisposable
    {
        private readonly List<(MethodBase target, MethodInfo patch)> patches = new();

        /// <summary>
        /// The harmony instance.
        /// </summary>
        protected Harmony Harmony { get; }

        /// <summary>
        /// Constructs an instance of the <see cref="Patcher"/> class.
        /// </summary>
        /// <param name="harmony">The harmony instance to use.</param>
        protected Patcher(Harmony harmony)
        {
            this.Harmony = harmony;
        }

        /// <inheritdoc cref="ISetup.Setup"/>
        public abstract void Setup();

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public virtual void Dispose()
        {
            // Remove all patches
            foreach (var (target, patch) in this.patches)
            {
                this.Harmony.Unpatch(target, patch);
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a patch for a target method.
        /// </summary>
        /// <param name="target">The target method to patch.</param>
        /// <param name="prefix">The prefix to apply, if any.</param>
        /// <param name="postfix">The postfix to apply, if any.</param>
        /// <param name="transpiler">The transpiler to apply, if any.</param>
        /// <param name="finalizer">The finalizer to apply, if any.</param>
        protected void Patch(
            MethodBase target,
            HarmonyMethod? prefix = null,
            HarmonyMethod? postfix = null,
            HarmonyMethod? transpiler = null,
            HarmonyMethod? finalizer = null
        )
        {
            var patch = this.Harmony.Patch(target, prefix, postfix, transpiler, finalizer);
            this.patches.Add((target, patch));
        }
    }
}
