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

namespace CopperStill {
    internal sealed class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            ModPatches.AdjustPricing.Register(helper);
            ModPatches.JuniperBerry.Register(helper);
            ModPatches.ModifyBundle.Register(helper);
            ModPatches.TipsyBuff.Register(helper);
        }
    }
}
