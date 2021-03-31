/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Harmony;
using ImJustMatt.Common.Patches;
using StardewModdingAPI;

namespace ImJustMatt.SlimeFramework.Framework.Patches
{
    internal class ObjectPatches : BasePatch<SlimeFramework>
    {
        public ObjectPatches(IMod mod, HarmonyInstance harmony) : base(mod, harmony)
        {
        }

        private static void DayUpdate()
        {
            
        }
    }
}