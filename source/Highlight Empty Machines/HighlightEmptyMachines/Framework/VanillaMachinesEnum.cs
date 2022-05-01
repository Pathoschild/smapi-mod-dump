/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/HighlightEmptyMachines
**
*************************************************/

using AtraBase.Toolkit.StringHandler;

namespace HighlightEmptyMachines.Framework;

/// <summary>
/// Enum for all the vanilla machines that require input.
/// </summary>
/// <remarks>Positive numbers refer to Big Craftables, negative ordinary SObjects.</remarks>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Should be obvious enough.")]
public enum VanillaMachinesEnum
{
    BeeHouse = 10,
    Keg = 12,
    Furnace = 13,
    PreservesJar = 15,
    CheesePress = 16,
    Loom = 17,
    OilMaker = 19,
    RecyclingMachine = 20,
    Crystalarium = 21,
    MayonnaiseMachine = 24,
    SeedMaker = 25,
    BoneMill = 90,
    Incubator = 101,
    CharcoalKiln = 114,
    SlimeIncubator = 156,
    SlimeEggPress = 158,
    Cask = 163,
    GeodeCrusher = 182,
    WoodChipper = 211,
    OstrichIncubator = 254,
    Deconstructor = 265,
    CrabPot = -710,
}

/// <summary>
/// Holds extension methods against this enum.
/// </summary>
internal static class VanillaMachinesEnumExtensions
{
    /// <summary>
    /// Tries to find the correct translation string for this machine.
    /// </summary>
    /// <param name="machine">VanillaMachineEnum.</param>
    /// <returns>A string, hopefully the translation.</returns>
    internal static string GetBestTranslatedString(this VanillaMachinesEnum machine)
    {
        if (machine < 0 && Game1.objectInformation.TryGetValue(-(int)machine, out string? val))
        {
            if (val.SpanSplit('/').TryGetAtIndex(SObject.objectInfoDisplayNameIndex, out SpanSplitEntry translatedName))
            {
                return translatedName;
            }
        }
        else if (machine > 0 && Game1.bigCraftablesInformation.TryGetValue((int)machine, out string? value))
        {
            int index = value.LastIndexOf('/');
            if (index >= 0)
            {
                return value[(index + 1)..];
            }
        }
        return ModEntry.TranslationHelper.Get($"{machine}.title");
    }
}