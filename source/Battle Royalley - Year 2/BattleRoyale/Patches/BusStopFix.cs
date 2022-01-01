/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace BattleRoyale.Patches
{
    class BusStopFix : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(BusStop), "resetLocalState");

        public static void Postfix(BusStop __instance)
        {
            ModEntry.BRGame.Helper.Reflection.GetField<Vector2>(__instance, "busPosition").SetValue(new Vector2(-1000f, -1000f));
            ModEntry.BRGame.Helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "busDoor").SetValue(null);
        }
    }
}
