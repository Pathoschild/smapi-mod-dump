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
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewRoguelike.Patches
{
    internal class PerformActionPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "performAction");

        public static bool Prefix(GameLocation __instance, ref bool __result, string action, Farmer who, Location tileLocation)
        {
            if (action is not null && who.IsLocalPlayer)
            {
                bool result = false;
                string[] actionParams = action.Split(' ');
                switch (actionParams[0])
                {
                    case "Buy":
                        __result = __instance.openShopMenu(actionParams[1]);
                        return false;
                }

                if (__instance is MineShaft mine)
                {
                    result = Merchant.PerformAction(mine, actionParams[0], Game1.player, tileLocation);
                    if (!result && ChallengeFloor.IsChallengeFloor(mine))
                        result = ChallengeFloor.PerformAction(mine, action, Game1.player, tileLocation);

                    if (result)
                    {
                        __result = true;
                        return false;
                    }
                }
                else if (__instance is Mine lobby)
                {
                    result = Roguelike.PerformAction(lobby, actionParams[0], Game1.player, tileLocation);
                }

                if (result)
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
