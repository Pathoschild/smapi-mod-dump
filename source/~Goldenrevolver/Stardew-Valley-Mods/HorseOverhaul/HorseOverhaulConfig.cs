/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewModdingAPI;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using System;

    public enum SaddleBagOption
    {
        Disabled = 0,
        Green = 1,
        Brown = 2,
        Horsemanship_Brown = 3,
        Horsemanship_Beige = 4
    }

    public enum WaterOption
    {
        Trough = 0,
        Bucket = 1,
        All = 2
    }

    public enum SaddleBagUnlockConditionOption
    {
        None = 0,
        Buy_From_Animal_Shop = 1
    }

    public enum WarpHorseFluteRequirementOption
    {
        None = 0,
        In_Inventory = 1,
        Owned = 2
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class HorseOverhaulConfig
    {
        private static readonly KeybindList HorseMenuKeyDefault = KeybindList.Parse("H, LeftStick+DPadUp");

        private static readonly KeybindList PetMenuKeyDefault = KeybindList.Parse("P, LeftStick+DPadDown");

        private static readonly KeybindList AlternateSaddleBagAndFeedKeyDefault = KeybindList.Parse("LeftStick");

        public bool ThinHorse { get; set; } = true;

        public bool MovementSpeed { get; set; } = true;

        public float MaxMovementSpeedBonus { get; set; } = 2.5f;

        public bool SaddleBag { get; set; } = true;

        public string SaddleBagUnlockCondition { get; set; } = SaddleBagUnlockConditionOption.None.ToString();

        public int SaddleBagUnlockPrice { get; set; } = 10000;

        public string VisibleSaddleBags { get; set; } = SaddleBagOption.Green.ToString();

        public bool FixHorseEmotePosition { get; set; } = true;

        public bool Petting { get; set; } = true;

        public bool Water { get; set; } = true;

        public string PreferredWaterContainer { get; set; } = WaterOption.Trough.ToString();

        public bool HorseHeater { get; set; } = true;

        public bool EnableLimitedInteractionWhileRiding { get; set; } = true;

        public bool InteractWithForageWhileRiding { get; set; } = true;

        public bool InteractWithBushesWhileRiding { get; set; } = true;

        public bool InteractWithTappersWhileRiding { get; set; } = true;

        public bool InteractWithMushroomLogsAndBoxesWhileRiding { get; set; } = true;

        public bool InteractWithTreesWhileRiding { get; set; } = true;

        public bool InteractWithFruitTreesWhileRiding { get; set; } = true;

        public bool InteractWithTrashCansWhileRiding { get; set; } = false;

        public bool WarpHorseWithYou { get; set; } = true;

        public string WarpHorseFluteRequirement { get; set; } = WarpHorseFluteRequirementOption.None.ToString();

        public int MaximumWarpDetectionRange { get; set; } = 200;

        public bool WarpHorseWithFluteIgnoresRange { get; set; } = false;

        public bool HorseHoofstepEffects { get; set; } = true;

        public bool Feeding { get; set; } = true;

        public bool PetFeeding { get; set; } = true;

        public bool NewFoodSystem { get; set; } = true;

        public bool AllowMultipleFeedingsADay { get; set; } = false;

        public bool ShowHorseInfoInAnimalsMenu { get; set; } = true;

        public KeybindList HorseMenuKey { get; set; } = HorseMenuKeyDefault;

        public KeybindList PetMenuKey { get; set; } = PetMenuKeyDefault;

        public KeybindList AlternateSaddleBagAndFeedKey { get; set; } = AlternateSaddleBagAndFeedKeyDefault;

        public int MaximumSaddleBagAndFeedRange { get; set; } = 78;

        public bool DisableMainSaddleBagAndFeedKey { get; set; } = false;

        public bool DisableStableSpriteChanges { get; set; } = false;

        public bool DisableHorseSounds { get; set; } = false;

        public static void InvalidateCache(HorseOverhaul mod)
        {
            try
            {
                // currently, I'm always applying the saddle bag skillbook objects addition
                // mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Objects");

                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Powers");

                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Shops");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache on config change {e}");
            }
        }

        public static void VerifyConfigValues(HorseOverhaulConfig config, HorseOverhaul mod)
        {
            bool invalidConfig = false;

            if (config.SaddleBagUnlockPrice < 0)
            {
                config.SaddleBagUnlockPrice = 0;
                invalidConfig = true;
            }

            if (config.MaxMovementSpeedBonus < 0f)
            {
                config.MaxMovementSpeedBonus = 0f;
                invalidConfig = true;
            }

            if (config.MaximumSaddleBagAndFeedRange < 0)
            {
                config.MaximumSaddleBagAndFeedRange = 0;
                invalidConfig = true;
            }

            if (Enum.TryParse(config.SaddleBagUnlockCondition, true, out SaddleBagUnlockConditionOption saddleBagUnlock))
            {
                // reassign to ensure casing is correct
                config.SaddleBagUnlockCondition = saddleBagUnlock.ToString();
            }
            else
            {
                config.SaddleBagUnlockCondition = SaddleBagUnlockConditionOption.None.ToString();
                invalidConfig = true;
            }

            if (Enum.TryParse(config.VisibleSaddleBags, true, out SaddleBagOption saddleBag))
            {
                // reassign to ensure casing is correct
                config.VisibleSaddleBags = saddleBag.ToString();
            }
            else
            {
                config.VisibleSaddleBags = SaddleBagOption.Disabled.ToString();
                invalidConfig = true;
            }

            if (Enum.TryParse(config.PreferredWaterContainer, true, out WaterOption waterContainer))
            {
                // reassign to ensure casing is correct
                config.PreferredWaterContainer = waterContainer.ToString();
            }
            else
            {
                config.PreferredWaterContainer = WaterOption.Trough.ToString();
                invalidConfig = true;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(HorseOverhaulConfig config, HorseOverhaul mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: delegate
                {
                    // if the world is ready, then we are not in the main menu, so reset should only reset the keybindings
                    if (Context.IsWorldReady)
                    {
                        config.HorseMenuKey = HorseMenuKeyDefault;
                        config.PetMenuKey = PetMenuKeyDefault;
                        config.AlternateSaddleBagAndFeedKey = AlternateSaddleBagAndFeedKeyDefault;
                        config.DisableMainSaddleBagAndFeedKey = false;
                        config.MaximumSaddleBagAndFeedRange = 78;
                    }
                    else
                    {
                        config = new HorseOverhaulConfig();
                        InvalidateCache(mod);
                    }
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    InvalidateCache(mod);
                }
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            api.AddSectionTitle(manifest, () => "General", null);

            api.AddBoolOption(manifest, () => config.ThinHorse, (bool val) => config.ThinHorse = val, () => "Thin Horse", null);

            api.AddSectionTitle(manifest, () => "Saddle Bags", null);

            api.AddBoolOption(manifest, () => config.SaddleBag, (bool val) => config.SaddleBag = val, () => "Enable Saddle Bags", () => "If this is disabled, then none of the settings below do anything");

            api.AddTextOption(manifest, () => config.SaddleBagUnlockCondition, (string val) => config.SaddleBagUnlockCondition = val, () => "Saddle Bag Unlock Condition", null, Enum.GetNames(typeof(SaddleBagUnlockConditionOption)), (s) => s.Replace('_', ' '));
            api.AddNumberOption(manifest, () => config.SaddleBagUnlockPrice, (int val) => config.SaddleBagUnlockPrice = val, () => "Saddle Bag Unlock Price", null, 0);

            api.AddTextOption(manifest, () => config.VisibleSaddleBags, (string val) => config.VisibleSaddleBags = val, () => "Visible Saddle Bags", null, Enum.GetNames(typeof(SaddleBagOption)), (s) => s.Replace('_', ' '));

            api.AddSectionTitle(manifest, () => "Friendship", null);

            api.AddBoolOption(manifest, () => config.MovementSpeed, (bool val) => config.MovementSpeed = val, () => "Movement Speed (MS)", null);
            api.AddNumberOption(manifest, () => config.MaxMovementSpeedBonus, (float val) => config.MaxMovementSpeedBonus = val, () => "Maximum MS Bonus", null, 0);
            api.AddBoolOption(manifest, () => config.Petting, (bool val) => config.Petting = val, () => "Petting", null);
            api.AddBoolOption(manifest, () => config.Feeding, (bool val) => config.Feeding = val, () => "Feeding", null);
            api.AddBoolOption(manifest, () => config.HorseHeater, (bool val) => config.HorseHeater = val, () => "Heater", null);

            api.AddBoolOption(manifest, () => config.Water, (bool val) => config.Water = val,
                () => "Water", null);
            api.AddTextOption(manifest, () => config.PreferredWaterContainer, (string val) => config.PreferredWaterContainer = val,
                () => "Preferred Water Container", () => "If the current stable sprite has both a trough and a bucket, which container to fill. If it has only one, this option is ignored", Enum.GetNames(typeof(WaterOption)), (s) => s.Replace('_', ' '));
            api.AddBoolOption(manifest, () => config.DisableStableSpriteChanges, (bool val) => config.DisableStableSpriteChanges = val,
                () => "Disable Stable Sprite Changes", null);
            api.AddBoolOption(manifest, () => config.ShowHorseInfoInAnimalsMenu, (bool val) => config.ShowHorseInfoInAnimalsMenu = val,
                () => "Show Horse Info In Animals Menu", null);

            api.AddSectionTitle(manifest, () => "Interact While Riding", null);

            api.AddBoolOption(manifest, () => config.EnableLimitedInteractionWhileRiding, (bool val) => config.EnableLimitedInteractionWhileRiding = val,
                () => "Enable Limited Interaction", () => "If this is disabled, then none of the settings below do anything");
            api.AddBoolOption(manifest, () => config.InteractWithForageWhileRiding, (bool val) => config.InteractWithForageWhileRiding = val,
                () => "Interact With Forage", null);
            api.AddBoolOption(manifest, () => config.InteractWithBushesWhileRiding, (bool val) => config.InteractWithBushesWhileRiding = val,
                () => "Interact With Bushes", null);
            api.AddBoolOption(manifest, () => config.InteractWithTappersWhileRiding, (bool val) => config.InteractWithTappersWhileRiding = val,
                () => "Interact With Tappers", null);
            api.AddBoolOption(manifest, () => config.InteractWithMushroomLogsAndBoxesWhileRiding, (bool val) => config.InteractWithMushroomLogsAndBoxesWhileRiding = val,
                () => "Interact With Mushroom Log/Box", null);
            api.AddBoolOption(manifest, () => config.InteractWithTreesWhileRiding, (bool val) => config.InteractWithTreesWhileRiding = val,
                () => "Interact With Trees", null);
            api.AddBoolOption(manifest, () => config.InteractWithFruitTreesWhileRiding, (bool val) => config.InteractWithFruitTreesWhileRiding = val,
                () => "Interact With Fruit Trees", null);
            api.AddBoolOption(manifest, () => config.InteractWithTrashCansWhileRiding, (bool val) => config.InteractWithTrashCansWhileRiding = val,
                () => "Interact With Trash Cans", () => "This may cause issues with some trash can or event mods that don't expect you to be riding a horse, but should work with almost all of them");

            api.AddSectionTitle(manifest, () => "Warp Horse With You", null);

            api.AddBoolOption(manifest, () => config.WarpHorseWithYou, (bool val) => config.WarpHorseWithYou = val,
                () => "Enable Warp Horse With You", () => "When using a warp totem, obelisk or scepter, also teleport your horse with you if it's nearby");
            api.AddTextOption(manifest, () => config.WarpHorseFluteRequirement, (string val) => config.WarpHorseFluteRequirement = val,
                () => "Warp Horse Flute Requirement", () => "Whether you need the horse flute for 'Warp Horse With You'", Enum.GetNames(typeof(WarpHorseFluteRequirementOption)), (s) => s.Replace('_', ' '));
            api.AddNumberOption(manifest, () => config.MaximumWarpDetectionRange, (int val) => config.MaximumWarpDetectionRange = val,
                () => "Maximum Warp\nDetection Range", () => "How close the horse has to be to be considered nearby");
            api.AddBoolOption(manifest, () => config.WarpHorseWithFluteIgnoresRange, (bool val) => config.WarpHorseWithFluteIgnoresRange = val,
                () => "Warp Horse With Flute\nIgnores Range", () => "When using 'Warp Horse With You' while you have a horse flute, always teleport your horse, even if it's not nearby");

            api.AddSectionTitle(manifest, () => "Other", null);

            api.AddBoolOption(manifest, () => config.HorseHoofstepEffects, (bool val) => config.HorseHoofstepEffects = val, () => "Horse Hoofstep Effects", null);
            api.AddBoolOption(manifest, () => config.DisableHorseSounds, (bool val) => config.DisableHorseSounds = val, () => "Disable Horse Sounds", null);
            api.AddBoolOption(manifest, () => config.NewFoodSystem, (bool val) => config.NewFoodSystem = val, () => "New Food System", null);
            api.AddBoolOption(manifest, () => config.PetFeeding, (bool val) => config.PetFeeding = val, () => "Pet Feeding", null);
            api.AddBoolOption(manifest, () => config.AllowMultipleFeedingsADay, (bool val) => config.AllowMultipleFeedingsADay = val, () => "Allow Multiple Feedings A Day", null);
            api.AddBoolOption(manifest, () => config.FixHorseEmotePosition, (bool val) => config.FixHorseEmotePosition = val, () => "Fix Horse Emote Position", null);

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.AddSectionTitle(manifest, () => "Keybindings", null);

            api.AddKeybindList(manifest, () => config.HorseMenuKey, (KeybindList keybindList) => config.HorseMenuKey = keybindList, () => "Horse Menu Key");
            api.AddKeybindList(manifest, () => config.PetMenuKey, (KeybindList keybindList) => config.PetMenuKey = keybindList, () => "Pet Menu Key");
            api.AddKeybindList(manifest, () => config.AlternateSaddleBagAndFeedKey, (KeybindList keybindList) => config.AlternateSaddleBagAndFeedKey = keybindList, () => "Alternate Saddle Bag\nAnd Feed Key", () => "You can use this key instead of the tool use key (left click, X) to interact with your horse (except riding).");
            api.AddBoolOption(manifest, () => config.DisableMainSaddleBagAndFeedKey, (bool val) => config.DisableMainSaddleBagAndFeedKey = val, () => "Disable Main Saddle Bag\nAnd Feed Key", () => "Disables interacting with your horse (except riding) by using the tool use key (left click, X).");
            api.AddNumberOption(manifest, () => config.MaximumSaddleBagAndFeedRange, (int val) => config.MaximumSaddleBagAndFeedRange = val, () => "Maximum Saddle Bag\nAnd Feed Range", () => "You can decrease this if you want to be closer before you can interact with your horse (except riding). Increasing it may not do that much, since there are also other internal range conditions.");

            // if the world is ready, then we are not in the main menu
            api.AddParagraph(manifest, () => Context.IsWorldReady ? "(All other settings are available in the main menu GMCM)" : string.Empty);
        }
    }

    /// <summary>
    /// Extension methods for IGameContentHelper.
    /// </summary>
    public static class GameContentHelperExtensions
    {
        /// <summary>
        /// Invalidates both an asset and the locale-specific version of an asset.
        /// </summary>
        /// <param name="helper">The game content helper.</param>
        /// <param name="assetName">The (string) asset to invalidate.</param>
        /// <returns>if something was invalidated.</returns>
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
            => helper.InvalidateCache(assetName)
                | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}