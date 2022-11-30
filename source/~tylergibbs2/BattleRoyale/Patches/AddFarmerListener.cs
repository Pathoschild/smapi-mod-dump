/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Network;

namespace BattleRoyale.Patches
{
    class AddFarmerListener : Patch
    {
        //runs when a client joins
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Multiplayer), "addPlayer");

        public static bool Prefix(NetFarmerRoot f)
        {
            if (Game1.IsServer)
            {
                ModEntry.BRGame.ProcessPlayerJoin(f);

                return AutoKicker.ProcessPlayerJoin(f);
            }

            return true;
        }
    }
}
