/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/LessMiniShippingBin
**
*************************************************/

using HarmonyLib;
using StardewValley.Objects;

namespace LessMiniShippingBin;

/// <summary>
/// Patches against StardewValley.Objects.Chest.
/// </summary>
[HarmonyPatch(typeof(Chest))]
internal class ChestPatches
{
    /// <summary>
    /// Postfix against the chest capacity.
    /// </summary>
    /// <param name="__instance">The chest to look at.</param>
    /// <param name="__result">The requested size of the chest.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chest.GetActualCapacity))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    public static void PostfixActualCapacity(Chest __instance, ref int __result)
    {
        try
        {
            switch (__instance.SpecialChestType)
            {
                case Chest.SpecialChestTypes.MiniShippingBin:
                    __result = ModEntry.Config.MiniShippingCapacity;
                    break;
                case Chest.SpecialChestTypes.JunimoChest:
                    __result = ModEntry.Config.JuminoCapcaity;
                    break;
                default:
                    // do nothing.
                    break;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in overwriting {__instance.SpecialChestType} capacity\n\n{ex}", LogLevel.Error);
        }
    }
}