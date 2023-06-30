/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2022 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using static GMCMOptions.IGMCMOptionsAPI;

namespace GMCMOptions.Framework {
    /// <summary>
    /// Implementation of the <c cref="IGMCMOptionsAPI">IGMCMOptionsAPI</c>.
    /// </summary>
    public class API : IGMCMOptionsAPI, IObsoleteApiMethods {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly IModInfo ClientMod;
        private readonly IModRegistry modRegistry;
        private bool fixedHeight;
        public API(IModHelper helper, IMonitor monitor, IModInfo clientMod) {
            Helper = helper;
            this.Monitor = monitor;
            this.ClientMod = clientMod;
            this.modRegistry = helper.ModRegistry;
            IModInfo? gmcm = modRegistry.Get("spacechase0.GenericModConfigMenu");
            if (gmcm is null) {
                this.fixedHeight = false;
            } else {
                this.fixedHeight = gmcm.Manifest.Version.IsOlderThan(new SemanticVersion(1, 8, 2));
            }
        }

        private Action<T>? MakeChangeHandlerFailed<T>(IManifest mod, string fieldId, string err) {
            Monitor.Log($"There was a problem doing reflection black magic.  Some dynamic updates inside the GMCM menu may not work correctly.", LogLevel.Info);
            Monitor.Log($"  failure registering handler for {mod.Name} field {fieldId}: {err}", LogLevel.Debug);
            return null;
        }

        // provides glue to allow the callbacks registered with GMCM's OnFieldChanged to get updates from
        // the custom ComplexOptions.  (There's no way to do this with the ComplexOption API - clearly a
        // shortcoming in GMCM.)
        private Action<T>? MakeChangeHandler<T>(IManifest mod, GMCMAPI gmcm, string? fieldId) {
            if (fieldId is null) return null;

            // Here be dragons.

            //Monitor.Log($"Printing fields of {gmcm.GetType()}", LogLevel.Debug);
            //foreach (var f in gmcm.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)) {
            //    Monitor.Log($"{f}", LogLevel.Debug);
            //}
            //Monitor.Log($"Printing properties of {gmcm.GetType()}", LogLevel.Debug);
            //foreach (var p in gmcm.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)) {
            //    Monitor.Log($"{p}", LogLevel.Debug);
            //}
            //Monitor.Log($"Printing methods of {gmcm.GetType()}", LogLevel.Debug);
            //foreach (var m in gmcm.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)) {
            //    Monitor.Log($"{m}", LogLevel.Debug);
            //}

            var gmcmApi = Helper.Reflection.GetField<object>(gmcm, "__Target", false)?.GetValue();
            if (gmcmApi is null) return MakeChangeHandlerFailed<T>(mod, fieldId, $"getting __Target from pintail API type {gmcm}");
            var configManager = Helper.Reflection.GetField<object>(gmcmApi, "ConfigManager")?.GetValue();
            if (configManager is null) return MakeChangeHandlerFailed<T>(mod, fieldId, $"getting ConfigManager field from gmcmApi {gmcmApi}"); ;
            var modConfig = Helper.Reflection.GetMethod(configManager, "Get", false)?.Invoke<object>(new object[] { mod, false });
            if (modConfig is null) return MakeChangeHandlerFailed<T>(mod, fieldId, $"getting ModConfig object from ConfigManager"); ;
            return (T val) => {
                var handlers = Helper.Reflection.GetProperty<List<Action<string, object?>>>(modConfig, "ChangeHandlers", false);
                if (handlers is null) {
                    MakeChangeHandlerFailed<T>(mod, fieldId, $"getting ChangeHandlers property from ModConfig object {modConfig}");
                    return;
                }
                foreach (var handler in handlers.GetValue()) {
                    handler.Invoke(fieldId, val);
                }
            };
        }

        /// <inheritdoc/>
        public void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string>? tooltip = null, bool showAlpha = true,
            uint colorPickerStyle = 0, string? fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ColorPickerOption option = new ColorPickerOption(fixedHeight, getValue, setValue, showAlpha,
                (ColorPickerStyle)colorPickerStyle, MakeChangeHandler<Color>(mod, gmcm, fieldId));
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }

        /// <inheritdoc/>
        public void AddImageOption(IManifest mod,
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
                    int arrowLocation = (int)ImageOptionArrowLocation.Top,
                    int labelLocation = (int)ImageOptionLabelLocation.Top,
                    string? fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ImagePickerOption option = new ImagePickerOption(getValue, setValue, getMaxValue,
                maxImageHeight, maxImageWidth, drawImage, label,
                imageTooltipTitle: imageTooltipTitle,
                imageTooltipText: imageTooltipText,
                (ImagePickerOption.ArrowLocation)arrowLocation, (ImagePickerOption.LabelLocation)labelLocation,
                MakeChangeHandler<uint>(mod, gmcm, fieldId));
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }

        /// <inheritdoc/>
        public void AddImageOption(IManifest mod,
                                   Func<uint> getValue,
                                   Action<uint> setValue,
                                   Func<string> name,
                                   Func<(Func<String?> label, Texture2D sheet, Rectangle? sourceRect)[]> choices,
                                   Func<string>? tooltip = null,
                                   Func<uint, String?>? imageTooltipTitle = null,
                                   Func<uint, String?>? imageTooltipText = null,
                                   int arrowLocation = (int)ImageOptionArrowLocation.Top,
                                   int labelLocation = (int)ImageOptionLabelLocation.Top,
                                   string? fieldId = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            ImagePickerOption option = new ImagePickerOption(
                getValue: getValue,
                setValue: setValue,
                getMaxValue: () => (uint)choices().Length - 1,
                maxImageHeight: () => choices().Select(choice => choice.sourceRect?.Height ?? choice.sheet.Height).Max(),
                maxImageWidth: () => choices().Select(choice => choice.sourceRect?.Width ?? choice.sheet.Width).Max(),
                drawImage: (v, b, pos) => {
                    var allChoices = choices();
                    int maxHeight = allChoices.Select(choice => choice.sourceRect?.Height ?? choice.sheet.Height).Max();
                    int maxWidth = allChoices.Select(choice => choice.sourceRect?.Width ?? choice.sheet.Width).Max();
                    var choice = allChoices[v];
                    int height = choice.sourceRect?.Height ?? choice.sheet.Height;
                    int width = choice.sourceRect?.Width ?? choice.sheet.Width;
                    Vector2 centeredPos = new Vector2(pos.X + (maxWidth - width) / 2, pos.Y + (maxHeight - height) / 2);
                    b.Draw(choice.sheet, centeredPos, choice.sourceRect, Color.White);
                },
                label: (v) => choices()[v].label?.Invoke(),
                imageTooltipTitle: imageTooltipTitle,
                imageTooltipText: imageTooltipText,
                (ImagePickerOption.ArrowLocation)arrowLocation, (ImagePickerOption.LabelLocation)labelLocation,
                MakeChangeHandler<uint>(mod, gmcm, fieldId));
            gmcm.AddComplexOption(
                mod: mod,
                name: name,
                tooltip: tooltip,
                draw: option.Draw,
                height: option.Height,
                beforeMenuOpened: option.Reset,
                beforeSave: option.SaveChanges,
                afterReset: option.Reset,
                fieldId: fieldId);
        }

        /// <inheritdoc/>
        public void AddSimpleHorizontalSeparator(IManifest mod,
                                                 double widthFraction = 0.85,
                                                 int height = 3,
                                                 int padAbove = 0,
                                                 int padBelow = 0,
                                                 int alignment = 0,
                                                 Color? color = null,
                                                 Color? shadowColor = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            SeparatorOption option = new SeparatorOption(
                getWidth: () => widthFraction,
                height: height,
                padAbove: padAbove,
                padBelow: padBelow,
                alignment: alignment,
                getColor: color is null ? null : () => (Color)color,
                getShadowColor: shadowColor is null ? null : () => (Color)shadowColor
            );
            gmcm.AddComplexOption(
                mod: mod,
                name: () => "",
                draw: option.Draw,
                height: option.OptionHeight);
        }

        /// <inheritdoc/>
        public void AddHorizontalSeparator(IManifest mod,
                                           Func<double>? getWidthFraction = null,
                                           int height = 3,
                                           int padAbove = 0,
                                           int padBelow = 0,
                                           int alignment = 0,
                                           Func<Color>? getColor = null,
                                           Func<Color>? getShadowColor = null) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            SeparatorOption option = new SeparatorOption(
                getWidth: getWidthFraction,
                height: height,
                padAbove: padAbove,
                padBelow: padBelow,
                alignment: alignment,
                getColor: getColor,
                getShadowColor: getShadowColor
            );
            gmcm.AddComplexOption(
                mod: mod,
                name: () => "",
                draw: option.Draw,
                height: option.OptionHeight);
        }

        /// <inheritdoc/>
        public void AddDynamicParagraph(IManifest mod, string logName, Func<string> text, bool isStyledText) {
            var gmcm = modRegistry.GetApi<GMCMAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            var log = (string err, LogLevel level) => {
                Monitor.Log($"[{ClientMod.Manifest.Name}] dynamic paragraph \"{logName}\": {err}", level);
            };
            DynamicParagraphOption option = new DynamicParagraphOption(
                getText: text,
                textLayoutEngine: isStyledText ? new StyledTextLayoutEngine(log) : new GameTextLayoutEngine());
            gmcm.AddComplexOption(
                mod: mod,
                name: () => "",
                draw: option.Draw,
                height: option.OptionHeight);
        }

        void IObsoleteApiMethods.AddImageOption(IManifest mod, Func<uint> getValue, Action<uint> setValue, Func<string> name, Func<uint> getMaxValue, Func<int> maxImageHeight, Func<int> maxImageWidth, Action<uint, SpriteBatch, Vector2> drawImage, Func<string>? tooltip, Func<uint, string?>? label, int arrowLocation, int labelLocation, string? fieldId) {
            AddImageOption(mod, getValue, setValue, name, getMaxValue, maxImageHeight, maxImageWidth, drawImage, tooltip, label, null, null, arrowLocation, labelLocation, fieldId);
        }

        void IObsoleteApiMethods.AddImageOption(IManifest mod, Func<uint> getValue, Action<uint> setValue, Func<string> name, Func<(Func<string?> label, Texture2D sheet, Rectangle? sourceRect)[]> choices, Func<string>? tooltip, int arrowLocation, int labelLocation, string? fieldId) {
            AddImageOption(mod, getValue, setValue, name, choices, tooltip, null, null, arrowLocation, labelLocation, fieldId);
        }
    }
    /// <summary>
    /// The portion of the GMCM API that we need
    /// </summary>
    public interface GMCMAPI {
        // see https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string>? tooltip = null, Action? beforeMenuOpened = null, Action? beforeSave = null, Action? afterSave = null, Action? beforeReset = null, Action? afterReset = null, Action? beforeMenuClosed = null, Func<int>? height = null, string? fieldId = null);

    }

}
