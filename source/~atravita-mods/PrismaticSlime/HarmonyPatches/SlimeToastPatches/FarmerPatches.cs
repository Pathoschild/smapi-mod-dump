/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;

using HarmonyLib;

namespace PrismaticSlime.HarmonyPatches.SlimeToastPatches;

[HarmonyPatch(typeof(Farmer))]
internal static class FarmerPatches
{
    internal const int BuffId = 15157;

    [HarmonyPatch(nameof(Farmer.doneEating))]
    private static void Prefix(Farmer __instance)
    {
        if (!Utility.IsNormalObjectAtParentSheetIndex(__instance.itemToEat, ModEntry.PrismaticJellyToast))
        {
            return;
        }

        try
        {
            BuffEnum buffenum = BuffEnumExtensions.GetRandomBuff();
            Buff buff = buffenum.GetBuffOf(5, 2600, "Prismatic Toast", I18n.PrismaticJellyToast_Name());
            buff.which = BuffId;
            buff.sheetIndex = 0;
            buff.description = I18n.PrismaticJellyBuff_Description(buffenum.ToStringFast());

            Game1.buffsDisplay.addOtherBuff(buff);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to add prismatic toast buff\n\n{ex}", LogLevel.Error);
        }
    }
}
