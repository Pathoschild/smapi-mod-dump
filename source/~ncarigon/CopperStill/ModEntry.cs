/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace CopperStill {
    internal sealed class ModEntry : Mod {
        public static ModEntry? Instance { get; private set; }

        public static Harmony? ModHarmony { get; private set; }

        public override void Entry(IModHelper helper) {
            Instance = this;
            ModHarmony = new Harmony(helper.ModContent.ModID);
            Config.Register();
            ModPatches.AdjustPricing.Register();
            ModPatches.ModifyBundle.Register();
            ModPatches.MachineData.Register();
            ModPatches.ItemSpawner.Register();
            ModPatches.LegacyItemConverter.Register();
        }
    }
}
