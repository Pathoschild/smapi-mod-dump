/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using BiggerJunimoChest.Patches;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;

namespace BiggerJunimoChest
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;
        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            HarmonyPatcher.Apply(this,
                new ChestPatcher()
            );
        }
    }
}
