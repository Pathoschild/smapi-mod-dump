/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using LocationLayerTool.Patches;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace LocationLayerTool
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            HarmonyPatcher.Apply(this,
                new xTileLayerPatcher()
            );

            this.Helper.ConsoleCommands.Add("llt_adddummy", "", this.DoCommand);
        }

        private void DoCommand(string cmd, string[] args)
        {
            Game1.locations.Add(new GameLocation(this.Helper.Content.GetActualAssetKey("assets/Farm_overlay.tbin"), "Farm_overlay"));
            Game1.game1.parseDebugInput("warp Farm_overlay 39 31");
        }
    }
}
