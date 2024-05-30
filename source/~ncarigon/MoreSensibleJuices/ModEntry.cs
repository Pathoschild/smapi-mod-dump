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
using MoreSensibleJuices.Patches;

namespace MoreSensibleJuices {
    internal sealed class ModEntry : Mod {
        public static ModEntry? Instance;

        public Harmony? ModHarmony { get; private set; }

        public override void Entry(IModHelper helper) {
            Instance = this;
            ModHarmony = new Harmony(helper.ModContent.ModID);
            MachineData.Register();
            ItemSpawner.Register();
        }
    }
}