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

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal abstract class Patcher : IDisposable
    {
        private readonly List<(MethodBase target, MethodInfo patch)> patches = new();
        protected Harmony Harmony { get; }

        protected Patcher(Harmony harmony)
        {
            this.Harmony = harmony;
        }

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

        public virtual void Dispose()
        {
            foreach (var (target, patch) in this.patches)
            {
                this.Harmony.Unpatch(target, patch);
            }
        }
    }
}