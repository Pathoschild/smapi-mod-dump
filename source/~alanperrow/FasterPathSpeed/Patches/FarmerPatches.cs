/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace FasterPathSpeed.Patches
{
    public class FarmerPatches
    {
        public static void GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                FasterPathSpeed.PostGetFarmerMovementSpeed(__instance, ref __result);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(GetMovementSpeed_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
