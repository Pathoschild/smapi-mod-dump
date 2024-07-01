/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Interfaces;
using Common.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace AnythingAnywhere;
public sealed class ModConfig : IConfigurable
{
    // PLACING
    [DefaultValue(true, "Placing")]
    public bool EnablePlacing { get; set; }

    [DefaultValue(false, "Placing")]
    public bool EnablePlaceAnywhere { get; set; }

    [DefaultValue(false, "Placing")]
    public bool EnableRugRemovalBypass { get; set; }

    // BUILDING
    [DefaultValue(true, "Building")]
    public bool EnableBuilding { get; set; }

    [DefaultValue(false, "Building")]
    public bool EnableBuildAnywhere { get; set; }

    [DefaultValue(false, "Building")]
    public bool EnableInstantBuild { get; set; }

    [DefaultValue(false, "Building")]
    public bool EnableFreeBuild { get; set; }

    [DefaultValue(SButton.OemComma, "Building")]
    public KeybindList? BuildMenu { get; set; }

    [DefaultValue(SButton.OemPeriod, "Building")]
    public KeybindList? WizardBuildMenu { get; set; }

    [DefaultValue(SButton.LeftShift, "Building")]
    public KeybindList? BuildModifier { get; set; }

    [DefaultValue(false, "Building")]
    public bool EnableGreenhouse { get; set; }

    [DefaultValue(false, "Building")]
    public bool RemoveBuildConditions { get; set; }

    [DefaultValue(false, "Building")]
    public bool EnableBuildingIndoors { get; set; }

    [DefaultValue(false, "Building")]
    public bool BypassMagicInk { get; set; }

    [DefaultValue(null, "Building")]
    public List<string>? BlacklistedLocations { get; set; }

    // FARMING
    [DefaultValue(true, "Farming")]
    public bool EnableFarmingAnywhere { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableCropsAnytime { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableTreesAnytime { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableBushesAnytime { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableDiggingAll { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableFruitTreeTweaks { get; set; }

    [DefaultValue(false, "Farming")]
    public bool EnableWildTreeTweaks { get; set; }

    [DefaultValue(false, "Farming")]
    public bool ForceGreenhouseTreeSprite { get; set; }

    // House
    [DefaultValue(true, "House")]
    public bool DisableHardCodedWarp { get; set; }

    [DefaultValue(false, "House")]
    public bool InstantHomeUpgrade { get; set; }

    [DefaultValue(true, "House")]
    public bool UpgradeCabins { get; set; }

    [DefaultValue(true, "House")]
    public bool RenovateCabins { get; set; }

    [DefaultValue(SButton.None, "House")]
    public KeybindList? CabinMenuButton { get; set; }

    [DefaultValue(false, "House")]
    public bool EnableFreeHouseUpgrade { get; set; }

    [DefaultValue(false, "House")]
    public bool EnableFreeRenovations { get; set; }

    // Misc
    [DefaultValue(false, "Misc")]
    public bool EnableCaskFunctionality { get; set; }

    [DefaultValue(false, "Misc")]
    public bool EnableFreeCommunityUpgrade { get; set; }

    [DefaultValue(false, "Misc")]
    public bool EnableJukeboxFunctionality { get; set; }

    [DefaultValue(true, "Misc")]
    public bool EnableGoldClockAnywhere { get; set; }

    [DefaultValue(false, "Misc")]
    public bool MultipleMiniObelisks { get; set; }

    public ModConfig()
    {
        ConfigUtility.InitializeDefaultConfig(this);
    }
}