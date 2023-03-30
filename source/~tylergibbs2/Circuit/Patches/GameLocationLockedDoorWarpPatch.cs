/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp))]
    internal class GameLocationLockedDoorWarpPatch
    {
        public static bool Prefix(ref string[] actionParams)
        {
            if (!ModEntry.Instance.IsActiveForSave || !EventManager.EventIsActive(EventType.PoorService))
                return true;

            if (actionParams.Length >= 5)
            {
                int originalOpen = Convert.ToInt32(actionParams[4]);
                actionParams[4] = (originalOpen + 200).ToString();
            }

            return true;
        }
    }
}
