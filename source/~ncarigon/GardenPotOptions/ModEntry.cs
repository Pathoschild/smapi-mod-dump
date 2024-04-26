/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace GardenPotOptions {
    internal class ModEntry : Mod {
        public static ModEntry? Instance;

        public Config? ModConfig;

        public override void Entry(IModHelper helper) {
            Instance = this;
            Config.Register();
            Patches.Register();
            Events.Register();
        }
    }
}
