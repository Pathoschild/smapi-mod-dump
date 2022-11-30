/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using System;

namespace BattleRoyale.Patches
{
    class WarpFarmerListener : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Game1), "warpFarmer", new Type[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) });

        public static bool Prefix(ref LocationRequest locationRequest)
        {
            FarmerUtils.AddKnockbackImmunity();

            if (!SpectatorMode.InSpectatorMode)
            {
                if (locationRequest.Location.Name == "Tunnel")
                {
                    var m = Game1.player.mount;
                    Game1.player.mount = null;
                    Game1.warpFarmer("Desert", 46, 27, false);
                    Game1.player.mount = m;

                    return false;
                }
            }

            return true;
        }
    }
}
