/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutomateToolSwap
{
    internal static class ConfigSetup
    {
        internal static bool isTractorModInstalled;
        public static void SetupConfig(IModHelper helper, Mod modInstance)
        {

            ModConfig Config = ModEntry.Config;
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var i18n = helper.Translation;
            var ModManifest = modInstance.ModManifest;
            isTractorModInstalled = modInstance.Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod");

            if (configMenu == null)
                return;

            configMenu.Register(ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => helper.WriteConfig(Config)
            );

            /****
            ** MAIN SETTINGS
            ****/
            configMenu.AddPageLink(ModManifest, "MainID", () => i18n.Get("config.mainPage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.mainPage.paragraph"));
            configMenu.AddPage(ModManifest, "MainID", () => i18n.Get("config.mainPage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.useCustomSwapKey.name"),
                tooltip: () => i18n.Get("config.useCustomSwapKey.tooltip"),
                getValue: () => Config.UseDifferentSwapKey,
                setValue: isEnabled => Config.UseDifferentSwapKey = isEnabled
            );
            configMenu.AddKeybindList(ModManifest,
                name: () => i18n.Get("config.SwapKey.name"),
                tooltip: () => i18n.Get("config.SwapKey.tooltip"),
                getValue: () => Config.SwapKey,
                setValue: keybinds => Config.SwapKey = keybinds
            );
            configMenu.AddKeybindList(ModManifest,
                name: () => i18n.Get("config.ToggleKey.name"),
                tooltip: () => i18n.Get("config.ToggleKey.tooltip"),
                getValue: () => Config.ToggleKey,
                setValue: keybinds => Config.ToggleKey = keybinds
            );
            configMenu.AddKeybindList(ModManifest,
                name: () => i18n.Get("config.LastToolKey.name"),
                tooltip: () => i18n.Get("config.LastToolKey.tooltip"),
                getValue: () => Config.LastToolKey,
                setValue: keybinds => Config.LastToolKey = keybinds
            );
            configMenu.AddTextOption(ModManifest,
                name: () => i18n.Get("config.DetectionMethod.name"),
                tooltip: () => i18n.Get("config.detectionMethod.tooltip"),
                allowedValues: new string[] {
                    "Cursor",
                    "Player"
                },
                formatAllowedValue: (string val) =>
                {
                    //construct the game's translated display name for each entry
                    switch (val)
                    {
                        case "Cursor":
                            return i18n.Get("config.DetectionMethod.cursor");
                        case "Player":
                            return i18n.Get("config.DetectionMethod.player");
                        default:
                            return null;
                    }
                },
                getValue: () => Config.DetectionMethod,
                setValue: method => Config.DetectionMethod = method
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AutoReturnToLastTool.name"),
                tooltip: () => i18n.Get("config.AutoReturnToLastTool.tooltip"),
                getValue: () => Config.AutoReturnToLastTool,
                setValue: isEnabled => Config.AutoReturnToLastTool = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** WEAPONS PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "WeaponsID", () => i18n.Get("config.weaponsPage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.weaponsPage.paragraph"));
            configMenu.AddPage(ModManifest, "WeaponsID", () => i18n.Get("config.weaponsPage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WeaponOnMonsters.name"),
                tooltip: () => i18n.Get("config.WeaponOnMonsters.tooltip"),
                getValue: () => Config.WeaponOnMonsters,
                setValue: isEnabled => Config.WeaponOnMonsters = isEnabled
                );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AlternativeWeaponOnMonsters.name"),
                tooltip: () => i18n.Get("config.AlternativeWeaponOnMonsters.tooltip"),
                getValue: () => Config.AlternativeWeaponOnMonsters,
                setValue: isEnabled => Config.AlternativeWeaponOnMonsters = isEnabled
            );
            configMenu.AddNumberOption(ModManifest,
                name: () => i18n.Get("config.MonsterRangeDetection.name"),
                tooltip: () => i18n.Get("config.MonsterRangeDetection.tooltip"),
                getValue: () => Config.MonsterRangeDetection,
                setValue: value => Config.MonsterRangeDetection = value,
                min: 1,
                max: 10
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WeaponForMineBarrels.name"),
                tooltip: () => i18n.Get("config.WeaponForMineBarrels.tooltip"),
                getValue: () => Config.WeaponForMineBarrels,
                setValue: isEnabled => Config.WeaponForMineBarrels = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.IgnoreCrabs.name"),
                tooltip: () => i18n.Get("config.IgnoreCrabs.tooltip"),
                getValue: () => Config.IgnoreCrabs,
                setValue: isEnabled => Config.IgnoreCrabs = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** PICKAXE PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "PickaxeID", () => i18n.Get("config.pickaxePage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.pickaxePage.paragraph"));
            configMenu.AddPage(ModManifest, "PickaxeID", () => i18n.Get("config.pickaxePage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.PickaxeForStoneAndOres.name"),
                tooltip: () => i18n.Get("config.PickaxeForStoneAndOres.tooltip"),
                getValue: () => Config.PickaxeForStoneAndOres,
                setValue: isEnabled => Config.PickaxeForStoneAndOres = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.PickaxeForBoulders.name"),
                tooltip: () => i18n.Get("config.PickaxeForBoulders.tooltip"),
                getValue: () => Config.PickaxeForBoulders,
                setValue: isEnabled => Config.PickaxeForBoulders = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
               name: () => i18n.Get("config.PickaxeOverWateringCan.name"),
               tooltip: () => i18n.Get("config.PickaxeOverWateringCan.tooltip"),
               getValue: () => Config.PickaxeOverWateringCan,
               setValue: isEnabled => Config.PickaxeOverWateringCan = isEnabled
           );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** AXE PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "AxeID", () => i18n.Get("config.axePage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.axePage.paragraph"));
            configMenu.AddPage(ModManifest, "AxeID", () => i18n.Get("config.axePage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AxeForTwigs.name"),
                tooltip: () => i18n.Get("config.AxeForTwigs.tooltip"),
                getValue: () => Config.AxeForTwigs,
                setValue: isEnabled => Config.AxeForTwigs = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AxeForTrees.name"),
                tooltip: () => i18n.Get("config.AxeForTrees.tooltip"),
                getValue: () => Config.AxeForTrees,
                setValue: isEnabled => Config.AxeForTrees = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AxeForFruitTrees.name"),
                tooltip: () => i18n.Get("config.AxeForFruitTrees.tooltip"),
                getValue: () => Config.AxeForFruitTrees,
                setValue: isEnabled => Config.AxeForFruitTrees = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AxeForStumpsAndLogs.name"),
                tooltip: () => i18n.Get("config.AxeForStumpsAndLogs.tooltip"),
                getValue: () => Config.AxeForStumpsAndLogs,
                setValue: isEnabled => Config.AxeForStumpsAndLogs = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AxeForGiantCrops.name"),
                tooltip: () => i18n.Get("config.AxeForGiantCrops.tooltip"),
                getValue: () => Config.AxeForGiantCrops,
                setValue: isEnabled => Config.AxeForGiantCrops = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.IgnoreGrowingTrees.name"),
                tooltip: () => i18n.Get("config.IgnoreGrowingTrees.tooltip"),
                getValue: () => Config.IgnoreGrowingTrees,
                setValue: isEnabled => Config.IgnoreGrowingTrees = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** HOE PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "HoeID", () => i18n.Get("config.hoePage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.hoePage.paragraph"));
            configMenu.AddPage(ModManifest, "HoeID", () => i18n.Get("config.hoePage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.HoeForArtifactSpots.name"),
                tooltip: () => i18n.Get("config.HoeForArtifactSpots.tooltip"),
                getValue: () => Config.HoeForArtifactSpots,
                setValue: isEnabled => Config.HoeForArtifactSpots = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.HoeForGingerCrop.name"),
                tooltip: () => i18n.Get("config.HoeForGingerCrop.tooltip"),
                getValue: () => Config.HoeForGingerCrop,
                setValue: isEnabled => Config.HoeForGingerCrop = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.HoeForDiggableSoil.name"),
                tooltip: () => i18n.Get("config.HoeForDiggableSoil.tooltip"),
                getValue: () => Config.HoeForDiggableSoil,
                setValue: isEnabled => Config.HoeForDiggableSoil = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** SCYTHE PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "ScytheID", () => i18n.Get("config.scythePage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.scythePage.paragraph"));
            configMenu.AddPage(ModManifest, "ScytheID", () => i18n.Get("config.scythePage.title"));
            /*Options*/

            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForWeeds.name"),
                tooltip: () => i18n.Get("config.ScytheForWeeds.tooltip"),
                getValue: () => Config.ScytheForWeeds,
                setValue: isEnabled => Config.ScytheForWeeds = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForBushes.name"),
                tooltip: () => i18n.Get("config.ScytheForBushes.tooltip"),
                getValue: () => Config.ScytheForBushes,
                setValue: isEnabled => Config.ScytheForBushes = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForMossOnTrees.name"),
                tooltip: () => i18n.Get("config.ScytheForMossOnTrees.tooltip"),
                getValue: () => Config.ScytheForMossOnTrees,
                setValue: isEnabled => Config.ScytheForMossOnTrees = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForCrops.name"),
                tooltip: () => i18n.Get("config.ScytheForCrops.tooltip"),
                getValue: () => Config.ScytheForCrops,
                setValue: isEnabled => Config.ScytheForCrops = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForGrass.name"),
                tooltip: () => i18n.Get("config.ScytheForGrass.tooltip"),
                getValue: () => Config.ScytheForGrass,
                setValue: isEnabled => Config.ScytheForGrass = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ScytheForForage.name"),
                tooltip: () => i18n.Get("config.ScytheForForage.tooltip"),
                getValue: () => Config.ScytheForForage,
                setValue: isEnabled => Config.ScytheForForage = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** WATERING CAN PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "WaterinCanID", () => i18n.Get("config.waterincanPage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.waterincanPage.paragraph"));
            configMenu.AddPage(ModManifest, "WaterinCanID", () => i18n.Get("config.waterincanPage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WateringCanForGardenPot.name"),
                tooltip: () => i18n.Get("config.WateringCanForGardenPot.tooltip"),
                getValue: () => Config.WateringCanForGardenPot,
                setValue: isEnabled => Config.WateringCanForGardenPot = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WateringCanForUnwateredCrop.name"),
                tooltip: () => i18n.Get("config.WateringCanForUnwateredCrop.tooltip"),
                getValue: () => Config.WateringCanForUnwateredCrop,
                setValue: isEnabled => Config.WateringCanForUnwateredCrop = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WateringCanForPetBowl.name"),
                tooltip: () => i18n.Get("config.WateringCanForPetBowl.tooltip"),
                getValue: () => Config.WateringCanForPetBowl,
                setValue: isEnabled => Config.WateringCanForPetBowl = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WateringCanForWater.name"),
                tooltip: () => i18n.Get("config.WateringCanForWater.tooltip"),
                getValue: () => Config.WateringCanForWater,
                setValue: isEnabled => Config.WateringCanForWater = isEnabled
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));


            /****
            ** MACHINES PAGE
            ****/
            configMenu.AddPageLink(ModManifest, "MachinesID", () => i18n.Get("config.machinesPage.title"));
            configMenu.AddParagraph(ModManifest, () => i18n.Get("config.machinesPage.paragraph"));
            configMenu.AddPage(ModManifest, "MachinesID", () => i18n.Get("config.machinesPage.title"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.WoolForLoom.name"),
                tooltip: () => i18n.Get("config.WoolForLoom.tooltip"),
                getValue: () => Config.WoolForLoom,
                setValue: isEnabled => Config.WoolForLoom = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.BoneForBoneMill.name"),
                tooltip: () => i18n.Get("config.BoneForBoneMill.tooltip"),
                getValue: () => Config.BoneForBoneMill,
                setValue: isEnabled => Config.BoneForBoneMill = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.EggsForMayoMachine.name"),
                tooltip: () => i18n.Get("config.EggsForMayoMachine.tooltip"),
                getValue: () => Config.EggsForMayoMachine,
                setValue: isEnabled => Config.EggsForMayoMachine = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.MilkForCheesePress.name"),
                tooltip: () => i18n.Get("config.MilkForCheesePress.tooltip"),
                getValue: () => Config.MilkForCheesePress,
                setValue: isEnabled => Config.MilkForCheesePress = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.MineralsForCrystalarium.name"),
                tooltip: () => i18n.Get("config.MineralsForCrystalarium.tooltip"),
                getValue: () => Config.MineralsForCrystalarium,
                setValue: isEnabled => Config.MineralsForCrystalarium = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.OresForFurnace.name"),
                tooltip: () => i18n.Get("config.OresForFurnace.tooltip"),
                getValue: () => Config.OresForFurnace,
                setValue: isEnabled => Config.OresForFurnace = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.FishForSmoker.name"),
                tooltip: () => i18n.Get("config.FishForSmoker.tooltip"),
                getValue: () => Config.FishForSmoker,
                setValue: isEnabled => Config.FishForSmoker = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.FishForBaitMaker.name"),
                tooltip: () => i18n.Get("config.FishForBaitMaker.tooltip"),
                getValue: () => Config.FishForBaitMaker,
                setValue: isEnabled => Config.FishForBaitMaker = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.BaitForCrabPot.name"),
                tooltip: () => i18n.Get("config.BaitForCrabPot.tooltip"),
                getValue: () => Config.BaitForCrabPot,
                setValue: isEnabled => Config.BaitForCrabPot = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.swapForSeedMaker.name"),
                tooltip: () => i18n.Get("config.swapForSeedMaker.tooltip"),
                getValue: () => Config.SwapForSeedMaker,
                setValue: isEnabled => Config.SwapForSeedMaker = isEnabled
            );
            configMenu.AddTextOption(ModManifest,
                name: () => i18n.Get("config.SwapForKegs.name"),
                tooltip: () => i18n.Get("config.SwapForKegs.tooltip"),
                allowedValues: new string[] {
                    "None",
                    "Fruit",
                    "Vegetable",
                    "Both"
                },
                formatAllowedValue: (string val) =>
                {
                    //construct the game's translated display name for each entry
                    switch (val)
                    {
                        case "None":
                            return i18n.Get("config.KegsAndJar.none");
                        case "Fruit":
                            return i18n.Get("config.KegsAndJar.fruit");
                        case "Vegetable":
                            return i18n.Get("config.KegsAndJar.vegetable");
                        case "Both":
                            return i18n.Get("config.KegsAndJar.both");
                        default:
                            return null;
                    }
                },
                getValue: () => Config.SwapForKegs,
                setValue: type => Config.SwapForKegs = type
            );
            configMenu.AddTextOption(ModManifest,
                name: () => i18n.Get("config.SwapForPreservesJar.name"),
                tooltip: () => i18n.Get("config.SwapForPreservesJar.tooltip"),
                allowedValues: new string[] {
                    "None",
                    "Fruit",
                    "Vegetable",
                    "Both"
                },
                formatAllowedValue: (string val) =>
                {
                    //construct the game's translated display name for each entry
                    switch (val)
                    {
                        case "None":
                            return i18n.Get("config.KegsAndJar.none");
                        case "Fruit":
                            return i18n.Get("config.KegsAndJar.fruit");
                        case "Vegetable":
                            return i18n.Get("config.KegsAndJar.vegetable");
                        case "Both":
                            return i18n.Get("config.KegsAndJar.both");
                        default:
                            return null;
                    }
                },
                getValue: () => Config.SwapForPreservesJar,
                setValue: type => Config.SwapForPreservesJar = type
            );
            configMenu.AddPage(ModManifest, String.Empty, () => i18n.Get("config.goBack"));

            /****
            ** Other Options
            ****/
            configMenu.AddSectionTitle(ModManifest, () => i18n.Get("config.otherOptions.text"));
            /*Options*/
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.SeedForTilledDirt.name"),
                tooltip: () => i18n.Get("config.SeedForTilledDirt.tooltip"),
                getValue: () => Config.SeedForTilledDirt,
                setValue: isEnabled => Config.SeedForTilledDirt = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.FertilizerForCrops.name"),
                tooltip: () => i18n.Get("config.FertilizerForCrops.tooltip"),
                getValue: () => Config.FertilizerForCrops,
                setValue: isEnabled => Config.FertilizerForCrops = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.FishingRodOnWater.name"),
                tooltip: () => i18n.Get("config.FishingRodOnWater.tooltip"),
                getValue: () => Config.FishingRodOnWater,
                setValue: isEnabled => Config.FishingRodOnWater = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AnyToolForWeeds.name"),
                tooltip: () => i18n.Get("config.AnyToolForWeeds.tooltip"),
                getValue: () => Config.AnyToolForWeeds,
                setValue: isEnabled => Config.AnyToolForWeeds = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.AnyToolForSupplyCrates.name"),
                tooltip: () => i18n.Get("config.AnyToolForSupplyCrates.tooltip"),
                getValue: () => Config.AnyToolForSupplyCrates,
                setValue: isEnabled => Config.AnyToolForSupplyCrates = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.PanForPanningSpots.name"),
                tooltip: () => i18n.Get("config.PanForPanningSpots.tooltip"),
                getValue: () => Config.PanForPanningSpots,
                setValue: isEnabled => Config.PanForPanningSpots = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.HayForFeedingBench.name"),
                tooltip: () => i18n.Get("config.HayForFeedingBench.tooltip"),
                getValue: () => Config.HayForFeedingBench,
                setValue: isEnabled => Config.HayForFeedingBench = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.MilkPailForCowsAndGoats.name"),
                tooltip: () => i18n.Get("config.MilkPailForCowsAndGoats.tooltip"),
                getValue: () => Config.MilkPailForCowsAndGoats,
                setValue: isEnabled => Config.MilkPailForCowsAndGoats = isEnabled
            );
            configMenu.AddBoolOption(ModManifest,
                name: () => i18n.Get("config.ShearsForSheeps.name"),
                tooltip: () => i18n.Get("config.ShearsForSheeps.tooltip"),
                getValue: () => Config.ShearsForSheeps,
                setValue: isEnabled => Config.ShearsForSheeps = isEnabled
            );



            // Add the Tractor settings
            if (isTractorModInstalled)
            {
                configMenu.AddSectionTitle(ModManifest,
                    text: () => i18n.Get("config.tractor.text")
                );

                configMenu.AddBoolOption(ModManifest,
                    name: () => i18n.Get("config.disableTractorSwap.name"),
                    tooltip: () => i18n.Get("config.disableTractorSwap.tooltip"),
                    getValue: () => Config.DisableTractorSwap,
                    setValue: isEnabled => Config.DisableTractorSwap = isEnabled
                );
            }
        }
    }
}