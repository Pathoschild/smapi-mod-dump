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
    using System;

    public enum SaddleBagOption
    {
        Disabled = 0,
        Green = 1,
        Brown = 2,
        Horsemanship_Brown = 3,
        Horsemanship_Beige = 4
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

        public float MaxMovementSpeedBonus { get; set; } = 3f;

        public bool SaddleBag { get; set; } = true;

        public string VisibleSaddleBags { get; set; } = SaddleBagOption.Green.ToString();

        public bool Petting { get; set; } = true;

        public bool Water { get; set; } = true;

        public bool HorseHeater { get; set; } = true;

        public bool EnableLimitedInteractionWhileRiding { get; set; } = true;

        public bool InteractWithForageWhileRiding { get; set; } = true;

        public bool InteractWithBushesWhileRiding { get; set; } = true;

        public bool InteractWithTappersWhileRiding { get; set; } = true;

        public bool InteractWithTreesWhileRiding { get; set; } = true;

        public bool InteractWithFruitTreesWhileRiding { get; set; } = true;

        public bool HorseHoofstepEffects { get; set; } = true;

        public bool Feeding { get; set; } = true;

        public bool PetFeeding { get; set; } = true;

        public bool NewFoodSystem { get; set; } = true;

        public bool AllowMultipleFeedingsADay { get; set; } = false;

        public KeybindList HorseMenuKey { get; set; } = HorseMenuKeyDefault;

        public KeybindList PetMenuKey { get; set; } = PetMenuKeyDefault;

        public KeybindList AlternateSaddleBagAndFeedKey { get; set; } = AlternateSaddleBagAndFeedKeyDefault;

        public bool DisableMainSaddleBagAndFeedKey { get; set; } = false;

        public bool DisableStableSpriteChanges { get; set; } = false;

        public bool DisableHorseSounds { get; set; } = false;

        public static void VerifyConfigValues(HorseOverhaulConfig config, HorseOverhaul mod)
        {
            bool invalidConfig = false;

            if (config.MaxMovementSpeedBonus < 0f)
            {
                config.MaxMovementSpeedBonus = 0f;
                invalidConfig = true;
            }

            if (Enum.TryParse(config.VisibleSaddleBags, true, out SaddleBagOption res))
            {
                // reassign to ensure casing is correct
                config.VisibleSaddleBags = res.ToString();
            }
            else
            {
                config.VisibleSaddleBags = SaddleBagOption.Disabled.ToString();
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
                    }
                    else
                    {
                        config = new HorseOverhaulConfig();
                    }
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                }
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            api.AddSectionTitle(manifest, () => "General", null);

            api.AddBoolOption(manifest, () => config.ThinHorse, (bool val) => config.ThinHorse = val, () => "Thin Horse", null);
            api.AddBoolOption(manifest, () => config.SaddleBag, (bool val) => config.SaddleBag = val, () => "Saddle Bags", null);
            api.AddTextOption(manifest, () => config.VisibleSaddleBags.ToString(), (string val) => config.VisibleSaddleBags = val, () => "Visible Saddle Bags", null, Enum.GetNames(typeof(SaddleBagOption)), (s) => s.Replace('_', ' '));

            api.AddSectionTitle(manifest, () => "Friendship", null);

            api.AddBoolOption(manifest, () => config.MovementSpeed, (bool val) => config.MovementSpeed = val, () => "Movement Speed (MS)", null);
            api.AddNumberOption(manifest, () => config.MaxMovementSpeedBonus, (float val) => config.MaxMovementSpeedBonus = val, () => "Maximum MS Bonus", null, 0);
            api.AddBoolOption(manifest, () => config.Petting, (bool val) => config.Petting = val, () => "Petting", null);
            api.AddBoolOption(manifest, () => config.Water, (bool val) => config.Water = val, () => "Water", null);
            api.AddBoolOption(manifest, () => config.Feeding, (bool val) => config.Feeding = val, () => "Feeding", null);
            api.AddBoolOption(manifest, () => config.HorseHeater, (bool val) => config.HorseHeater = val, () => "Heater", null);

            api.AddSectionTitle(manifest, () => "Interact While Riding", null);

            api.AddBoolOption(manifest, () => config.EnableLimitedInteractionWhileRiding, (bool val) => config.EnableLimitedInteractionWhileRiding = val,
                () => "Enable Limited Interaction", () => "If this is disabled, then none of the settings below do anything");
            api.AddBoolOption(manifest, () => config.InteractWithForageWhileRiding, (bool val) => config.InteractWithForageWhileRiding = val,
                () => "Interact With Forage", null);
            api.AddBoolOption(manifest, () => config.InteractWithBushesWhileRiding, (bool val) => config.InteractWithBushesWhileRiding = val,
                () => "Interact With Bushes", null);
            api.AddBoolOption(manifest, () => config.InteractWithTappersWhileRiding, (bool val) => config.InteractWithTappersWhileRiding = val,
                () => "Interact With Tappers", null);
            api.AddBoolOption(manifest, () => config.InteractWithTreesWhileRiding, (bool val) => config.InteractWithTreesWhileRiding = val,
                () => "Interact With Trees", null);
            api.AddBoolOption(manifest, () => config.InteractWithFruitTreesWhileRiding, (bool val) => config.InteractWithFruitTreesWhileRiding = val,
                () => "Interact With Fruit Trees", null);

            api.AddSectionTitle(manifest, () => "Other", null);

            api.AddBoolOption(manifest, () => config.HorseHoofstepEffects, (bool val) => config.HorseHoofstepEffects = val, () => "Horse Hoofstep Effects", null);
            api.AddBoolOption(manifest, () => config.DisableHorseSounds, (bool val) => config.DisableHorseSounds = val, () => "Disable Horse Sounds", null);
            api.AddBoolOption(manifest, () => config.NewFoodSystem, (bool val) => config.NewFoodSystem = val, () => "New Food System", null);
            api.AddBoolOption(manifest, () => config.PetFeeding, (bool val) => config.PetFeeding = val, () => "Pet Feeding", null);
            api.AddBoolOption(manifest, () => config.AllowMultipleFeedingsADay, (bool val) => config.AllowMultipleFeedingsADay = val, () => "Allow Multiple Feedings A Day", null);
            api.AddBoolOption(manifest, () => config.DisableStableSpriteChanges, (bool val) => config.DisableStableSpriteChanges = val, () => "Disable Stable Sprite Changes", null);

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.AddSectionTitle(manifest, () => "Keybindings", null);

            api.AddKeybindList(manifest, () => config.HorseMenuKey, (KeybindList keybindList) => config.HorseMenuKey = keybindList, () => "Horse Menu Key");
            api.AddKeybindList(manifest, () => config.PetMenuKey, (KeybindList keybindList) => config.PetMenuKey = keybindList, () => "Pet Menu Key");
            api.AddKeybindList(manifest, () => config.AlternateSaddleBagAndFeedKey, (KeybindList keybindList) => config.AlternateSaddleBagAndFeedKey = keybindList, () => "Alternate Saddle Bag\nAnd Feed Key");
            api.AddBoolOption(manifest, () => config.DisableMainSaddleBagAndFeedKey, (bool val) => config.DisableMainSaddleBagAndFeedKey = val, () => "Disable Main Saddle Bag\nAnd Feed Key", null);
        }
    }
}