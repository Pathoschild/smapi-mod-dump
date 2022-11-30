/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using Force.DeepCloner;
using StardewModdingAPI;

namespace InventoryRandomizer;

internal class GenericModConfigMenuHelper
{
    internal static ModConfig CachedConfig;

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            Globals.Manifest,
            () =>
                {
                    Globals.Config = new ModConfig();
                    CachedConfig = Globals.Config.ShallowClone();
                },
            () =>
                {
                    Globals.Helper.WriteConfig(Globals.Config);

                    TimeManager.ResetTimer();
                    if (Context.IsWorldReady && Globals.Config.ChatMessageAlerts && ConfigTimeHasChanged())
                    {
                        ChatManager.DisplayCurrentConfigMessage();
                    }

                    CachedConfig = Globals.Config.ShallowClone();
                }
            );

        /* General */

        Globals.GmcmApi.AddSectionTitle(
            Globals.Manifest,
            () => "General"
        );

        Globals.GmcmApi.AddBoolOption(
            Globals.Manifest,
            name: () => "Chat Message Alerts",
            tooltip: () => "Receive periodic alerts on the time until inventory randomization occurs.",
            getValue: () => Globals.Config.ChatMessageAlerts,
            setValue: val => Globals.Config.ChatMessageAlerts = val
        );

        Globals.GmcmApi.AddBoolOption(
            Globals.Manifest,
            name: () => "Play Sound on Randomization",
            tooltip: () => "Play a sound when the inventory is randomized.",
            getValue: () => Globals.Config.PlaySoundOnRandomization,
            setValue: val => Globals.Config.PlaySoundOnRandomization = val
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Seconds Until Randomization",
            tooltip: () => "Time in seconds until the inventory is randomized again.",
            getValue: () => Globals.Config.SecondsUntilInventoryRandomization,
            setValue: val => Globals.Config.SecondsUntilInventoryRandomization = val
        );

        Globals.GmcmApi.AddSectionTitle(
            Globals.Manifest,
            () => "Probability Weights"
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Recipe Chance",
            tooltip: () =>
                "Chance for a recipe to show up instead of an item, if that item has a valid recipe.\nNOTE: Learning a recipe removes it from the pool.",
            getValue: () => Globals.Config.RecipeChance,
            setValue: val => Globals.Config.RecipeChance = val,
            min: 0f,
            max: 1f,
            interval: 0.01f
        );

        Globals.GmcmApi.AddParagraph(
            Globals.Manifest,
            () =>
                "This section controls the weights of each category, or how likely they are to be chosen. Higher values are more likely to be chosen.");

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Craftables Weight",
            tooltip: () => "How likely BigCraftables (i.e. Furnaces, Heaters, Scarecrows, etc.) are to be chosen.",
            getValue: () => Globals.Config.BigCraftablesWeight,
            setValue: val => Globals.Config.BigCraftablesWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Boots Weight",
            tooltip: () => "How likely Boots are to be chosen.",
            getValue: () => Globals.Config.BootsWeight,
            setValue: val => Globals.Config.BootsWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Clothing Weight",
            tooltip: () => "How likely Clothing is to be chosen.",
            getValue: () => Globals.Config.ClothingWeight,
            setValue: val => Globals.Config.ClothingWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Furniture Weight",
            tooltip: () => "How likely Furniture is to be chosen.",
            getValue: () => Globals.Config.FurnitureWeight,
            setValue: val => Globals.Config.FurnitureWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Hats Weight",
            tooltip: () => "How likely Hats are to be chosen.",
            getValue: () => Globals.Config.HatsWeight,
            setValue: val => Globals.Config.HatsWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Objects Weight",
            tooltip: () => "How likely Objects (food, fish, crops, quest items, etc.) are to be chosen.",
            getValue: () => Globals.Config.ObjectsWeight,
            setValue: val => Globals.Config.ObjectsWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Weapons Weight",
            tooltip: () => "How likely Weapons are to be chosen.",
            getValue: () => Globals.Config.WeaponsWeight,
            setValue: val => Globals.Config.WeaponsWeight = val,
            min: 0
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Tools Weight",
            tooltip: () => "How likely Tools are to be chosen.",
            getValue: () => Globals.Config.ToolsWeight,
            setValue: val => Globals.Config.ToolsWeight = val,
            min: 0
        );
    }

    private static bool ConfigTimeHasChanged() =>
        CachedConfig is null || CachedConfig.SecondsUntilInventoryRandomization !=
        Globals.Config.SecondsUntilInventoryRandomization;
}
