/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRainbowCursor
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace RainbowCursor {
    public class ModConfig {
        private const int paletteHeight = 100;
        private const int paletteWidth = 150;

        public bool Enabled { get; set; } = true;
        private float _speed = 1.0f;
        public float Speed {
            get => _speed;
            set => _speed = Math.Clamp(value, 0f, 10f);
        }
        public bool Fade { get; set; } = true;
        public string Palette { get; set; } = "prismatic";

        public ModConfig() {
            // default values all specified above
        }

        internal static void RegisterGMCM(ModEntry theMod) {
            var mod = theMod.ModManifest;
            var gmcm = theMod.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm is null) return;
            ModConfig uncommittedConfig = (ModConfig)theMod.config.MemberwiseClone();
            gmcm.Register(
                mod: mod,
                reset: () => {
                    theMod.config = new ModConfig();
                    uncommittedConfig = (ModConfig)theMod.config.MemberwiseClone();
                },
                save: () => theMod.Helper.WriteConfig(theMod.config),
                titleScreenOnly: false);
            gmcm.OnFieldChanged(mod: mod,
                onChange: (string str, object obj) => {
                    //theMod.Monitor.Log($"OnFieldChanged {str} {obj}", LogLevel.Debug);
                    switch (str) {
                        case "enabled":
                            uncommittedConfig.Enabled = (bool)obj;
                            break;
                        case "speed":
                            uncommittedConfig.Speed = (float)obj;
                            break;
                        case "fade":
                            uncommittedConfig.Fade = (bool)obj;
                            break;
                        case "palette-id":
                            uncommittedConfig.Palette = (string)obj;
                            break;
                        case "palette-idx":
                            uncommittedConfig.Palette = theMod.paletteRegistry.Get((uint)obj).Id;
                            break;
                        default:
                            // shouldn't happen
                            theMod.Monitor.Log($"Internal error: got OnFieldChanged for {str}");
                            break;
                    }

                });

            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Enabled,
                tooltip: I18n.Config_Enabled_Tooltip,
                getValue: () => theMod.config.Enabled,
                setValue: (v) => theMod.config.Enabled = v,
                fieldId: "enabled");
            gmcm.AddNumberOption(
                mod: mod,
                name: I18n.Config_Speed,
                tooltip: I18n.Config_Speed_Tooltip,
                getValue: () => theMod.config.Speed,
                setValue: (v) => theMod.config.Speed = v,
                min: 0f,
                max: 10f,
                interval: 0.1f,
                fieldId: "speed");
            gmcm.AddBoolOption(
                mod: mod,
                name: I18n.Config_Fade,
                tooltip: I18n.Config_Fade_Tooltip,
                getValue: () => theMod.config.Fade,
                setValue: (v) => theMod.config.Fade = v,
                fieldId: "fade");

            Action<uint, SpriteBatch, Vector2> drawPalette = (idx, sb, pos) => {
                ColorPalette p = theMod.paletteRegistry.Get(idx);
                int stripeWidth = paletteWidth / p.Colors.Count;
                int leftPx = (int)pos.X;
                int top = (int)pos.Y;
                foreach (Color c in p.Colors) {
                    sb.Draw(StardewValley.Game1.staminaRect, new Rectangle(leftPx, top, stripeWidth, paletteHeight), c);
                    leftPx += stripeWidth;
                }
            };

            var gmcmOpt = theMod.Helper.ModRegistry.GetApi<GMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (gmcmOpt is not null) {
                gmcmOpt.AddImageOption(
                    mod: mod,
                    name: I18n.Config_Palette,
                    tooltip: I18n.Config_Palette_Tooltip,
                    getValue: () => theMod.paletteRegistry.IndexOf(theMod.config.Palette),
                    setValue: (idx) => { theMod.config.Palette = theMod.paletteRegistry.Get(idx).Id; theMod.RefreshCurrentPalette(); },
                    getMaxValue: () => (uint)theMod.paletteRegistry.Count - 1,
                    maxImageHeight: () => paletteHeight,
                    maxImageWidth: () => paletteWidth,
                    label: (idx) => theMod.paletteRegistry.Get(idx).GetName(),
                    imageTooltipTitle: (idx) => theMod.paletteRegistry.Get(idx).GetTitle?.Invoke(),
                    imageTooltipText: (idx) => theMod.paletteRegistry.Get(idx).GetDescription?.Invoke(),
                    arrowLocation: 0, // sides
                    fieldId: "palette-idx",
                    drawImage: drawPalette);
            } else {
                var gmcmOpt_1_4 = theMod.Helper.ModRegistry.GetApi<GMCMOptionsAPI_1_4>("jltaylor-us.GMCMOptions");
                if (gmcmOpt_1_4 is not null) {
                    gmcmOpt_1_4.AddImageOption(
                        mod: mod,
                        name: I18n.Config_Palette,
                        tooltip: I18n.Config_Palette_Tooltip,
                        getValue: () => theMod.paletteRegistry.IndexOf(theMod.config.Palette),
                        setValue: (idx) => { theMod.config.Palette = theMod.paletteRegistry.Get(idx).Id; theMod.RefreshCurrentPalette(); },
                        getMaxValue: () => (uint)theMod.paletteRegistry.Count - 1,
                        maxImageHeight: () => paletteHeight,
                        maxImageWidth: () => paletteWidth,
                        label: (idx) => theMod.paletteRegistry.Get(idx).GetName(),
                        arrowLocation: 0, // sides
                        fieldId: "palette-idx",
                        drawImage: drawPalette);
                } else {
                    // this is pretty lame, since it can't add new known IDs to the allowed values
                    // list after initialization
                    List<string> knownIds = new();
                    foreach (ColorPalette p in theMod.paletteRegistry) {
                        knownIds.Add(p.Id);
                    }
                    gmcm.AddTextOption(
                        mod: mod,
                        name: I18n.Config_Palette,
                        tooltip: I18n.Config_Palette_Tooltip,
                        getValue: () => theMod.config.Palette,
                        setValue: (v) => { theMod.config.Palette = v; theMod.RefreshCurrentPalette(); },
                        allowedValues: knownIds.ToArray(),
                        formatAllowedValue: (id) => theMod.paletteRegistry.Get(id).GetName(),
                        fieldId: "palette-id");
                    gmcm.AddParagraph(mod: mod, text: I18n.Config_InstallGmcmOptions);
                    theMod.Monitor.Log(I18n.Message_InstallGmcmOptions(), LogLevel.Info);
                }
            }
            gmcm.AddComplexOption(
                mod: mod,
                name: I18n.Config_Preview,
                draw: (SpriteBatch sb, Vector2 pos) => theMod.DrawPreview(sb, pos, uncommittedConfig)
                );
        }
    }

    // See https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs for full API
    public interface GenericModConfigMenuAPI {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null);
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string>? tooltip = null, Action? beforeMenuOpened = null, Action? beforeSave = null, Action? afterSave = null, Action? beforeReset = null, Action? afterReset = null, Action? beforeMenuClosed = null, Func<int>? height = null, string? fieldId = null);
    }

    public interface GMCMOptionsAPI_1_4 {
        void AddImageOption(IManifest mod,
                            Func<uint> getValue,
                            Action<uint> setValue,
                            Func<string> name,
                            Func<uint> getMaxValue,
                            Func<int> maxImageHeight,
                            Func<int> maxImageWidth,
                            Action<uint, SpriteBatch, Vector2> drawImage,
                            Func<string>? tooltip = null,
                            Func<uint, String?>? label = null,
                            int arrowLocation = -1, // top
                            int labelLocation = -1, // top
                            string? fieldId = null);
    }
    public interface GMCMOptionsAPI {
        void AddImageOption(IManifest mod,
                            Func<uint> getValue,
                            Action<uint> setValue,
                            Func<string> name,
                            Func<uint> getMaxValue,
                            Func<int> maxImageHeight,
                            Func<int> maxImageWidth,
                            Action<uint, SpriteBatch, Vector2> drawImage,
                            Func<string>? tooltip = null,
                            Func<uint, String?>? label = null,
                            Func<uint, String?>? imageTooltipTitle = null,
                            Func<uint, String?>? imageTooltipText = null,
                            int arrowLocation = -1, // top
                            int labelLocation = -1, // top
                            string? fieldId = null);
    }
}

