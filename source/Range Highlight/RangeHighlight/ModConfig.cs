/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020-2022 Jamie Taylor
using System;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace RangeHighlight {
    internal class ModConfig {
        public bool ShowJunimoRange { get; set; } = true;
        public bool ShowSprinklerRange { get; set; } = true;
        public bool ShowScarecrowRange { get; set; } = true;
        public bool ShowBeehouseRange { get; set; } = true;
        public bool ShowBombRange { get; set; } = true;

        public bool ShowOtherSprinklersWhenHoldingSprinkler { get; set; } = true;
        public bool ShowOtherScarecrowsWhenHoldingScarecrow { get; set; } = true;
        public bool ShowOtherBeehousesWhenHoldingBeehouse { get; set; } = false;

        public bool showHeldBombRange { get; set; } = true;
        public bool showPlacedBombRange { get; set; } = true;
        public bool showBombInnerRange { get; set; } = false;
        public bool showBombOuterRange { get; set; } = true;
        public KeybindList ShowAllRangesKey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);
        public KeybindList ShowSprinklerRangeKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public KeybindList ShowScarecrowRangeKey { get; set; } = KeybindList.ForSingle(SButton.O);
        public KeybindList ShowBeehouseRangeKey { get; set; } = KeybindList.ForSingle(SButton.H);
        public KeybindList ShowJunimoRangeKey { get; set; } = KeybindList.ForSingle(SButton.J);
        public bool hotkeysToggle { get; set; } = false;

        public Color JunimoRangeTint { get; set; } = Color.White * 0.7f;
        public Color SprinklerRangeTint { get; set; } = new Color(0.6f, 0.6f, 0.9f, 0.7f);
        public Color ScarecrowRangeTint { get; set; } = new Color(0.6f, 1.0f, 0.6f, 0.7f);
        public Color BeehouseRangeTint { get; set; } = new Color(1.0f, 1.0f, 0.6f, 0.7f);
        public Color BombRangeTint { get; set; } = new Color(1.0f, 0.5f, 0.5f, 0.6f);
        public Color BombInnerRangeTint { get; set; } = new Color(8.0f, 0.7f, 0.5f, 0.1f);
        public Color BombOuterRangeTint { get; set; } = new Color(9.0f, 0.7f, 0.5f, 0.8f);

        public static void RegisterGMCM(ModEntry theMod) {
            var mod = theMod.ModManifest;
            var defaultColorPickerStyle = (uint)(GMCMOptionsAPI.ColorPickerStyle.AllStyles | GMCMOptionsAPI.ColorPickerStyle.RadioChooser);
            var gmcm = theMod.helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm is null) return;
            gmcm.Register(
                mod: mod,
                reset: () => theMod.config = new ModConfig(),
                save: () => theMod.Helper.WriteConfig(theMod.config),
                titleScreenOnly: true);

            var gmcmOpt = theMod.helper.ModRegistry.GetApi<GMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (gmcmOpt is null) {
                theMod.Monitor.Log(I18n.Message_InstallGmcmOptions(theMod.ModManifest.Name), LogLevel.Info);
                gmcm.AddParagraph(mod, I18n.Config_InstallGmcmOptions);
            }

            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_HotkeysToggle,
                tooltip: I18n.Config_HotkeysToggle_Tooltip,
                getValue: () => theMod.config.hotkeysToggle,
                setValue: (v) => theMod.config.hotkeysToggle = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_ShowAllKey,
                tooltip: I18n.Config_ShowAllKey_Tooltip,
                getValue: () => theMod.config.ShowAllRangesKey,
                setValue: (v) => theMod.config.ShowAllRangesKey = v);

            // Junimo Huts
            gmcm.AddSectionTitle(mod, I18n.Config_Junimo);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Junimo_Enable_Tooltip,
                getValue: () => theMod.config.ShowJunimoRange,
                setValue: (v) => theMod.config.ShowJunimoRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Junimo_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowJunimoRangeKey,
                setValue: (v) => theMod.config.ShowJunimoRangeKey = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Junimo_Tint_Tooltip,
                getValue: () => theMod.config.JunimoRangeTint,
                setValue: (v) => theMod.config.JunimoRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);


            // Sprinklers
            gmcm.AddSectionTitle(mod, I18n.Config_Sprinkler);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Sprinkler_Enable_Tooltip,
                getValue: () => theMod.config.ShowSprinklerRange,
                setValue: (v) => theMod.config.ShowSprinklerRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Sprinkler_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowSprinklerRangeKey,
                setValue: (v) => theMod.config.ShowSprinklerRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Sprinkler_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherSprinklersWhenHoldingSprinkler,
                setValue: (v) => theMod.config.ShowOtherSprinklersWhenHoldingSprinkler = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Sprinkler_Tint_Tooltip,
                getValue: () => theMod.config.SprinklerRangeTint,
                setValue: (v) => theMod.config.SprinklerRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);


            // Scarecrows
            gmcm.AddSectionTitle(mod, I18n.Config_Scarecrow);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Scarecrow_Enable_Tooltip,
                getValue: () => theMod.config.ShowScarecrowRange,
                setValue: (v) => theMod.config.ShowScarecrowRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Scarecrow_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowScarecrowRangeKey,
                setValue: (v) => theMod.config.ShowScarecrowRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Scarecrow_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherScarecrowsWhenHoldingScarecrow,
                setValue: (v) => theMod.config.ShowOtherScarecrowsWhenHoldingScarecrow = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Scarecrow_Tint_Tooltip,
                getValue: () => theMod.config.ScarecrowRangeTint,
                setValue: (v) => theMod.config.ScarecrowRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);

            // Beehouses
            gmcm.AddSectionTitle(mod, I18n.Config_Beehouse);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Beehouse_Enable_Tooltip,
                getValue: () => theMod.config.ShowBeehouseRange,
                setValue: (v) => theMod.config.ShowBeehouseRange = v);
            gmcm.AddKeybindList(
                mod: mod,
                name: I18n.Config_Hotkey,
                tooltip: I18n.Config_Beehouse_Hotkey_Tooltip,
                getValue: () => theMod.config.ShowBeehouseRangeKey,
                setValue: (v) => theMod.config.ShowBeehouseRangeKey = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_ShowOthers,
                tooltip: I18n.Config_Beehouse_ShowOthers_Tooltip,
                getValue: () => theMod.config.ShowOtherBeehousesWhenHoldingBeehouse,
                setValue: (v) => theMod.config.ShowOtherBeehousesWhenHoldingBeehouse = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Beehouse_Tint_Tooltip,
                getValue: () => theMod.config.BeehouseRangeTint,
                setValue: (v) => theMod.config.BeehouseRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);

            // Bombs
            gmcm.AddSectionTitle(mod, I18n.Config_Bomb);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enable,
                tooltip: I18n.Config_Bomb_Enable_Tooltip,
                getValue: () => theMod.config.ShowBombRange,
                setValue: (v) => theMod.config.ShowBombRange = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Held,
                tooltip: I18n.Config_Bomb_Held_Tooltip,
                getValue: () => theMod.config.showHeldBombRange,
                setValue: (v) => theMod.config.showHeldBombRange = v);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Placed,
                tooltip: I18n.Config_Bomb_Placed_Tooltip,
                getValue: () => theMod.config.showPlacedBombRange,
                setValue: (v) => theMod.config.showPlacedBombRange = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Tint,
                tooltip: I18n.Config_Bomb_Tint_Tooltip,
                getValue: () => theMod.config.BombRangeTint,
                setValue: (v) => theMod.config.BombRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Inner_Enable,
                tooltip: I18n.Config_Bomb_Inner_Enable_Tooltip,
                getValue: () => theMod.config.showBombInnerRange,
                setValue: (v) => theMod.config.showBombInnerRange = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Bomb_Inner_Tint,
                tooltip: I18n.Config_Bomb_Inner_Tint_Tooltip,
                getValue: () => theMod.config.BombInnerRangeTint,
                setValue: (v) => theMod.config.BombInnerRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Bomb_Outer_Enable,
                tooltip: I18n.Config_Bomb_Outer_Enable_Tooltip,
                getValue: () => theMod.config.showBombOuterRange,
                setValue: (v) => theMod.config.showBombOuterRange = v);
            gmcmOpt?.AddColorOption(
                mod: mod,
                name: I18n.Config_Bomb_Outer_Tint,
                tooltip: I18n.Config_Bomb_Outer_Tint_Tooltip,
                getValue: () => theMod.config.BombOuterRangeTint,
                setValue: (v) => theMod.config.BombOuterRangeTint = v,
                colorPickerStyle: defaultColorPickerStyle);



        }
    }
    // See https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs for full API
    public interface GenericModConfigMenuAPI {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
    // See https://github.com/jltaylor-us/StardewGMCMOptions/blob/default/StardewGMCMOptions/IGMCMOptionsAPI.cs
    public interface GMCMOptionsAPI {
        void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string> tooltip = null, bool showAlpha = true, uint colorPickerStyle = 0, string fieldId = null);
        #pragma warning disable format
        [Flags]
        public enum ColorPickerStyle : uint {
            Default = 0,
            RGBSliders    = 0b00000001,
            HSVColorWheel = 0b00000010,
            HSLColorWheel = 0b00000100,
            AllStyles     = 0b11111111,
            NoChooser     = 0,
            RadioChooser  = 0b01 << 8,
            ToggleChooser = 0b10 << 8
        }
        #pragma warning restore format
    }
}
