/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using System;
using System.Collections.Generic;

namespace MushroomRancher
{
    internal class MushroomIncubator
    {
        public static readonly string mushroomIncubatorNonQID = $"{MushroomRancher.Mod?.ModManifest?.UniqueID}.MushroomIncubator";
        public static readonly string mushroomIncubatorQualifiedID = $"(BC){mushroomIncubatorNonQID}";

        internal static void AddIncubatorAssetChanges(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                    var recipeConfig = MushroomRancher.Config.IncubatorRecipe;
                    var recipeConfigUnlock = MushroomRancher.Config.IncubatorRecipeUnlock;

                    if (string.IsNullOrWhiteSpace(recipeConfig))
                    {
                        recipeConfig = MushroomRancherConfig.DefaultIncubatorRecipe;
                    }
                    else
                    {
                        recipeConfig = recipeConfig.Trim();
                    }
                    if (string.IsNullOrWhiteSpace(recipeConfigUnlock))
                    {
                        recipeConfigUnlock = MushroomRancherConfig.DefaultIncubatorRecipeUnlock;
                    }
                    else
                    {
                        recipeConfigUnlock = recipeConfigUnlock.Trim();
                    }

                    data[mushroomIncubatorNonQID] = $"{recipeConfig}/Home/{mushroomIncubatorNonQID}/true/{recipeConfigUnlock}/";
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, BigCraftableData> data = asset.AsDictionary<string, BigCraftableData>().Data;

                    var mushroomIncubator = new BigCraftableData
                    {
                        Name = mushroomIncubatorNonQID,
                        DisplayName = MushroomRancher.Mod.Helper.Translation.Get("MushroomIncubator.name"),
                        Description = MushroomRancher.Mod.Helper.Translation.Get("MushroomIncubator.description"),
                        Price = 0,
                        Fragility = 0,
                        CanBePlacedOutdoors = true,
                        CanBePlacedIndoors = false,
                        IsLamp = false,
                        Texture = MushroomRancher.Mod.MushroomIncubatorAssetPath,
                        SpriteIndex = 0,
                        ContextTags = null,
                        CustomFields = null
                    };

                    data[mushroomIncubatorNonQID] = mushroomIncubator;
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, MachineData> data = asset.AsDictionary<string, MachineData>().Data;

                    // most of the values of the mushroom incubator were set to be identical to the slime incubator
                    // in case the default values of these constructors change in the future, all values were defined

                    var mushroomIncubator = new MachineData
                    {
                        HasInput = false,
                        HasOutput = false,
                        InteractMethod = null,
                        PreventTimePass = null,
                        ReadyTimeModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                        InvalidItemMessage = null,
                        InvalidItemMessageCondition = null,
                        InvalidCountMessage = null,
                        WorkingEffects = null,
                        WorkingEffectChance = 0.33f,
                        AllowLoadWhenFull = false,
                        WobbleWhileWorking = MushroomRancher.Config.IncubatorWobblesWhileIncubating,
                        LightWhileWorking = null,
                        ShowNextIndexWhileWorking = false,
                        ShowNextIndexWhenReady = false,
                        AllowFairyDust = false,
                        IsIncubator = false, // this is not a bug, slime incubators are also not "incubators"
                        ClearContentsOvernightCondition = null,
                        StatsToIncrementWhenLoaded = null,
                        StatsToIncrementWhenHarvested = null,
                        CustomFields = null
                    };

                    bool coopmasterWouldResultInLessThanOneDay = MushroomRancher.Config.IncubatorDurationIsInDaysInsteadOfMinutes && MushroomRancher.Config.IncubatorDuration <= 1;

                    if (MushroomRancher.Config.IncubatorIsAffectedByCoopmaster && !coopmasterWouldResultInLessThanOneDay)
                    {
                        var coopmaster = new QuantityModifier
                        {
                            Id = "Coopmaster",
                            Condition = "PLAYER_HAS_PROFESSION Target 2",
                            Modification = QuantityModifier.ModificationType.Multiply,
                            Amount = MushroomRancher.Config.IncubatorDurationIsInDaysInsteadOfMinutes ? 0.45f : 0.5f,
                            RandomAmount = null
                        };

                        mushroomIncubator.ReadyTimeModifiers = new List<QuantityModifier>() { coopmaster };
                    }
                    else
                    {
                        mushroomIncubator.ReadyTimeModifiers = null;
                    }

                    var coinSound = new MachineSoundData
                    {
                        Id = "coin",
                        Delay = 0
                    };

                    var defaultMachineSpriteEffect = new MachineEffects
                    {
                        Id = "Default",
                        Condition = null,
                        Interval = -1,
                        Frames = null,
                        ShakeDuration = -1,
                        TemporarySprites = null,
                        Sounds = new List<MachineSoundData>() { coinSound }
                    };

                    mushroomIncubator.LoadEffects = new List<MachineEffects>()
                    {
                        defaultMachineSpriteEffect
                    };

                    int incubatorDuration = Math.Max(1, MushroomRancher.Config.IncubatorDuration);

                    var defaultOutput = new MachineOutputRule
                    {
                        Id = "Default",
                        UseFirstValidOutput = true,
                        MinutesUntilReady = MushroomRancher.Config.IncubatorDurationIsInDaysInsteadOfMinutes ? -1 : incubatorDuration,
                        DaysUntilReady = MushroomRancher.Config.IncubatorDurationIsInDaysInsteadOfMinutes ? incubatorDuration : -1,
                        RecalculateOnCollect = false
                    };

                    var redMushroomDisplayName = ItemRegistry.Create(MushroomRancher.redMushroomId).DisplayName;
                    var magmaCapDisplayName = ItemRegistry.Create(MushroomRancher.magmaCapId).DisplayName;

                    defaultOutput.InvalidCountMessage = MushroomRancher.Mod.Helper.Translation.Get("MushroomIncubator.mushroomWarning"
                        , new { redMushroomName = redMushroomDisplayName, magmaCapName = magmaCapDisplayName });

                    var redTrigger = CreateMachineOutputTriggerRule(MushroomRancher.redMushroomId);
                    var purpleTrigger = CreateMachineOutputTriggerRule(MushroomRancher.magmaCapId);

                    defaultOutput.Triggers = new List<MachineOutputTriggerRule> { redTrigger, purpleTrigger };

                    var defaultOutputItem = CreateMachineItemOutput("Default");
                    var purpleMushroomOutputItem = CreateMachineItemOutput("MagmaMushroom");

                    defaultOutput.OutputItem = new List<MachineItemOutput>() { purpleMushroomOutputItem, defaultOutputItem };

                    mushroomIncubator.OutputRules = new List<MachineOutputRule>() { defaultOutput };

                    if (MushroomRancher.Config.IncubatorAdditionalRequiredItemCount > 0)
                    {
                        int additionalItemCount = MushroomRancher.Config.IncubatorAdditionalRequiredItemCount;
                        string additionalItemID = MushroomRancher.Config.IncubatorAdditionalRequiredItemID ?? MushroomRancherConfig.DefaultIncubatorAdditionalRequiredItemID;

                        Item additionalItem = ItemRegistry.Create(additionalItemID, allowNull: true);
                        if (additionalItem == null)
                        {
                            additionalItemID = MushroomRancherConfig.DefaultIncubatorAdditionalRequiredItemID;
                        }
                        else
                        {
                            additionalItemID = additionalItem.QualifiedItemId;
                        }

                        var machineAdditionalConsumedItem = new MachineItemAdditionalConsumedItems
                        {
                            ItemId = additionalItemID,
                            RequiredCount = additionalItemCount
                        };

                        var additionalItemDisplayName = ItemRegistry.Create(machineAdditionalConsumedItem.ItemId).DisplayName;

                        machineAdditionalConsumedItem.InvalidCountMessage = MushroomRancher.Mod.Helper.Translation.Get("MushroomIncubator.additionalItemWarning"
                            , new { additionalItemName = additionalItemDisplayName, additionalItemCount = machineAdditionalConsumedItem.RequiredCount.ToString() });

                        mushroomIncubator.AdditionalConsumedItems = new List<MachineItemAdditionalConsumedItems>() { machineAdditionalConsumedItem };
                    }
                    else
                    {
                        mushroomIncubator.AdditionalConsumedItems = null;
                    }

                    data[mushroomIncubatorQualifiedID] = mushroomIncubator;
                });
            }
        }

        private static MachineOutputTriggerRule CreateMachineOutputTriggerRule(string requiredItemId)
        {
            return new MachineOutputTriggerRule
            {
                Id = "ItemPlacedInMachine",
                Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                RequiredItemId = requiredItemId,
                RequiredTags = null,
                RequiredCount = 1,
                Condition = null
            };
        }

        private static MachineItemOutput CreateMachineItemOutput(string outputID)
        {
            return new MachineItemOutput
            {
                CustomData = null,
                OutputMethod = null,
                CopyColor = false,
                CopyPrice = false,
                CopyQuality = false,
                PreserveType = null,
                PreserveId = null,
                IncrementMachineParentSheetIndex = outputID == "Default" ? 1 : 2,
                PriceModifiers = null,
                PriceModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                Condition = outputID == "Default" ? null : $"ITEM_ID Input {MushroomRancher.magmaCapId}",
                Id = outputID,
                ItemId = "DROP_IN",
                RandomItemId = null,
                MaxItems = null,
                MinStack = -1,
                MaxStack = -1,
                Quality = -1,
                ObjectInternalName = null,
                ObjectDisplayName = null,
                ToolUpgradeLevel = -1,
                IsRecipe = false,
                StackModifiers = null,
                StackModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                QualityModifiers = null,
                QualityModifierMode = QuantityModifier.QuantityModifierMode.Stack,
                PerItemCondition = null
            };
        }
    }
}