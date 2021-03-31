/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using Harmony;
using StardewModdingAPI;

namespace ImJustMatt.Common.Patches
{
    internal class Patcher
    {
        private readonly IMod _mod;
        private readonly string _uniqueId;

        internal Patcher(IMod mod)
        {
            _mod = mod;
            _uniqueId = mod.ModManifest.UniqueID;
        }

        internal void ApplyAll(params Type[] patchTypes)
        {
            var harmony = HarmonyInstance.Create(_uniqueId);
            foreach (var patchType in patchTypes)
            {
                Activator.CreateInstance(patchType, _mod, harmony);
            }
        }
    }
}