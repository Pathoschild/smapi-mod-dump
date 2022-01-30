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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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

    public interface IGenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc, Func<Vector2, object, object> widgetUpdate, Func<SpriteBatch, Vector2, object, object> widgetDraw, Action<object> onSave);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);

        void AddParagraph(IManifest mod, Func<string> text);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class HorseConfig
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

        public static void VerifyConfigValues(HorseConfig config, HorseOverhaul mod)
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

        public static void SetUpModConfigMenu(HorseConfig config, HorseOverhaul mod)
        {
            IGenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(
                manifest,
                delegate
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
                        config = new HorseConfig();
                    }
                },
                delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                }
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, true);

            api.RegisterLabel(manifest, "General", null);

            api.RegisterSimpleOption(manifest, "Thin Horse", null, () => config.ThinHorse, (bool val) => config.ThinHorse = val);
            api.RegisterSimpleOption(manifest, "Saddle Bags", null, () => config.SaddleBag, (bool val) => config.SaddleBag = val);
            api.RegisterChoiceOption(manifest, "Visible Saddle Bags", null, () => config.VisibleSaddleBags.ToString(), (string val) => config.VisibleSaddleBags = val, Enum.GetNames(typeof(SaddleBagOption)));

            api.RegisterLabel(manifest, "Friendship", null);

            api.RegisterSimpleOption(manifest, "Movement Speed (MS)", null, () => config.MovementSpeed, (bool val) => config.MovementSpeed = val);
            api.RegisterSimpleOption(manifest, "Maximum MS Bonus", null, () => config.MaxMovementSpeedBonus, (float val) => config.MaxMovementSpeedBonus = val);
            api.RegisterSimpleOption(manifest, "Petting", null, () => config.Petting, (bool val) => config.Petting = val);
            api.RegisterSimpleOption(manifest, "Water", null, () => config.Water, (bool val) => config.Water = val);
            api.RegisterSimpleOption(manifest, "Feeding", null, () => config.Feeding, (bool val) => config.Feeding = val);
            api.RegisterSimpleOption(manifest, "Heater", null, () => config.HorseHeater, (bool val) => config.HorseHeater = val);

            api.RegisterLabel(manifest, "Other", null);

            api.RegisterSimpleOption(manifest, "Horse Hoofstep Effects", null, () => config.HorseHoofstepEffects, (bool val) => config.HorseHoofstepEffects = val);
            api.RegisterSimpleOption(manifest, "Disable Horse Sounds", null, () => config.DisableHorseSounds, (bool val) => config.DisableHorseSounds = val);
            api.RegisterSimpleOption(manifest, "New Food System", null, () => config.NewFoodSystem, (bool val) => config.NewFoodSystem = val);
            api.RegisterSimpleOption(manifest, "Pet Feeding", null, () => config.PetFeeding, (bool val) => config.PetFeeding = val);
            api.RegisterSimpleOption(manifest, "Allow Multiple Feedings A Day", null, () => config.AllowMultipleFeedingsADay, (bool val) => config.AllowMultipleFeedingsADay = val);
            api.RegisterSimpleOption(manifest, "Disable Stable Sprite Changes", null, () => config.DisableStableSpriteChanges, (bool val) => config.DisableStableSpriteChanges = val);

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.RegisterLabel(manifest, "Keybindings", null);

            api.AddKeybindList(manifest, () => config.HorseMenuKey, (KeybindList keybindList) => config.HorseMenuKey = keybindList, () => "Horse Menu Key");
            api.AddKeybindList(manifest, () => config.PetMenuKey, (KeybindList keybindList) => config.PetMenuKey = keybindList, () => "Pet Menu Key");
            api.AddKeybindList(manifest, () => config.AlternateSaddleBagAndFeedKey, (KeybindList keybindList) => config.AlternateSaddleBagAndFeedKey = keybindList, () => "Alternate Saddle Bag\nAnd Feed Key");
            api.RegisterSimpleOption(manifest, "Disable Main Saddle Bag\nAnd Feed Key", null, () => config.DisableMainSaddleBagAndFeedKey, (bool val) => config.DisableMainSaddleBagAndFeedKey = val);
        }
    }
}